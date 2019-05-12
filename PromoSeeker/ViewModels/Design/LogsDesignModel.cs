using System;
using System.Collections.ObjectModel;

namespace PromoSeeker
{
    /// <summary>
    /// The design-time data model for a <see cref="LogsViewModel"/>.
    /// </summary>
    public class LogsDesignModel : LogsViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static LogsDesignModel Instance = new LogsDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LogsDesignModel()
        {
            Data = new ObservableCollection<Tuple<string, string, DateTime>>()
            {
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World", DateTime.Now),
                new Tuple<string, string, DateTime>("Sample Product", "Hello World. This is a longer, detailed message for the log entry. I wonder if this would fit nicely into the cell.", DateTime.Today),
            };
        }

        #endregion
    }
}
