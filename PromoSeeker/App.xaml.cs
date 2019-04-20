﻿using System;
using System.Windows;
using PromoSeeker.Core;

namespace PromoSeeker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Setup dependency injection
            DI.Setup();

            // Bind logger into the core solution
            CoreDI.Logger = DI.Logger;

            // Initialize UI components.
            DI.UIManager.Initialize();

            // Log startup
            DI.Logger.Info("Application started");

            // Load application content
            DI.Application.Load();
        }
    }
}