using System.Windows;

namespace CompomentLib.TimeEditor
{
    /// <summary>
    /// Provides data for the time editor change events. 
    /// </summary>
    public sealed class TimeEditorEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The action performed on the TimeEditor.
        /// </summary>
        public TimeEditorAction Action { get; private set; }

        /// <summary>
        /// The field on which the <see cref="Action" /> is performed
        /// </summary>
        public TimeEditorField ActiveField { get; private set; }
                
        /// <summary>
        /// Initializes a new instance of the TimeEditorEventArgs class.
        /// </summary>
        /// <param name="action">The action performed on the TimeEditor</param>
        /// <param name="activeField">The field on which the action was performed.</param>
        public TimeEditorEventArgs(TimeEditorAction action, TimeEditorField activeField)
        {
            Action = action;
            ActiveField = activeField;
        }
        
        /// <summary>
        /// Initializes a new instance of the TimeEditorEventArgs class, using the supplied routed event identifier.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the RoutedEventArgs class.</param>
        /// <param name="action">The action performed on the TimeEditor</param>
        /// <param name="activeField">The field on which the action was performed.</param>
        public TimeEditorEventArgs(RoutedEvent routedEvent, TimeEditorAction action, TimeEditorField activeField)
            : base(routedEvent)
        {
            Action = action;
            ActiveField = activeField;
        }
        
        /// <summary>
        /// Initializes a new instance of the TimeEditorEventArgs class, using the supplied routed event identifier, and
        /// providing the opportunity to declar a different source for the event.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the RoutedEventArgs class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This
        /// pre-populates the RoutedEventArgs.Source property.</param>
        /// <param name="action">The action performed on the TimeEditor</param>
        /// <param name="activeField">The field on which the action was performed.</param>
        public TimeEditorEventArgs(RoutedEvent routedEvent, object source, TimeEditorAction action, TimeEditorField activeField)
            : base(routedEvent, source)
        {
            Action = action;
            ActiveField = activeField;
        }
    }
}
