﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Due.Data;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Scheduler;

namespace Due
{
    public partial class App : Application
    {
        private static MainViewModel viewModel = null;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                    viewModel = new MainViewModel();

                return viewModel;
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public Context db;

        public static int DB_VERSION = 2;

        public object state;

        Color foregroundColor;
        Color backgroundColor;

        public static string JumpToView = "";

        PeriodicTask periodicTask;
        string periodicTaskName = "DueAgent";

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            this.foregroundColor = (Resources["PhoneForegroundBrush"] as SolidColorBrush).Color;
            this.backgroundColor = (Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color;
            this.OverrideColors();

            (Resources["PhoneSubtleBrush"] as SolidColorBrush).Color = Color.FromArgb(0x99, 0xFF, 0xFF, 0xFF); 

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            this.db = new Context();

            if (this.db.DatabaseExists() == false)
            {
                //FIRST LOGIN

                //Create the database
                this.db.CreateDatabase();
                this.db = new Context();

                //Sample data
                Todo i = new Todo
                {
                    Title = "This item it's due today",
                    DateInsert = DateTime.Today,
                    DueDate = DateTime.Today
                };

                Todo i2 = new Todo
                {
                    Title = "This should be done very soon",
                    DateInsert = DateTime.Now.AddMinutes(-40),
                    DueDate = DateTime.Today.AddDays(1)
                };

                Todo i3 = new Todo
                {
                    Title = "Take your time, there's no rush to do this",
                    DateInsert = DateTime.Today.AddDays(-5),
                    DueDate = DateTime.Today.AddDays(60)
                };

                db.todos.InsertOnSubmit(i);
                db.todos.InsertOnSubmit(i2);
                db.todos.InsertOnSubmit(i3);

                db.SubmitChanges();
            }
            else
            {
                this.UpdateLocalDatabase();

                //Delete old todo
                var x = (from Todo t in db.todos where t.Completed == true && t.DateInsert <= DateTime.Today.AddDays(-7) select t).ToList();
                if (x.Count > 0)
                {
                    db.todos.DeleteAllOnSubmit(x);
                    db.SubmitChanges();
                }
            }

            StartPeriodicAgent();
        }

        public void OverrideColors()
        {
            (Resources["PhoneForegroundBrush"] as SolidColorBrush).Color = Colors.Black;
            (Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color = Colors.White;
        }

        public void RestoreColors()
        {
            (Resources["PhoneForegroundBrush"] as SolidColorBrush).Color = this.foregroundColor;
            (Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color = this.backgroundColor;
        }

        private void UpdateLocalDatabase()
        {
            try
            {
                // Check whether a database update is needed.
                DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                bool dbUpdated = false;

                int SchemaVersion = dbUpdater.DatabaseSchemaVersion;

                if (SchemaVersion < 2)
                {
                    dbUpdater.AddColumn<Todo>("ManualDueDate");
                    dbUpdated = true;
                }

                if (dbUpdated)
                {
                    dbUpdater.DatabaseSchemaVersion = DB_VERSION;
                    dbUpdater.Execute();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

                DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                if (dbUpdater.DatabaseSchemaVersion == 0)
                {
                    try
                    {
                        dbUpdater.AddTable<Todo>();
                        dbUpdater.Execute();

                        this.UpdateLocalDatabase();
                    }
                    catch (Exception exc)
                    {
                        System.Diagnostics.Debug.WriteLine(exc.Message);
                    }
                }
            }
        }

        private void StartPeriodicAgent()
        {
            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "This task updates the live tile with your today's tasks.";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);

                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
    ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    //Nothing
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // Ensure that application state is restored appropriately
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            db.UpdateTileCount();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Ensure that required application state is persisted here.
            db.UpdateTileCount();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}