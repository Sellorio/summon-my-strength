namespace SummonMyStrength.Maui.Components.Common.DragDrop
{
    /// <summary>
    /// A service used to facilitate the drag-drop feature.
    /// </summary>
    public interface IDragDropService
    {
        /// <summary>
        /// Gets the group key relating to the object being dragged. Objects can only be dragged to groups with the same key.
        /// </summary>
        object ActiveGroupKey { get; }

        /// <summary>
        /// Gets the key being dragged at the moment.
        /// </summary>
        object ActiveKey { get; }

        /// <summary>
        /// Gets the width of the content being dragged. Ideally dropsites should scale to match the height
        /// when active.
        /// </summary>
        int DragContentWidth { get; }

        /// <summary>
        /// Gets the height of the content being dragged. Ideally dropsites should scale to match the height
        /// when active.
        /// </summary>
        int DragContentHeight { get; }

        /// <summary>
        /// Gets or sets the currently active dropsite which will be used if dragging is stopped.
        /// </summary>
        IDropsite Dropsite { get; set; }

        /// <summary>
        /// Begins a drag-drop operation for the given object in the given grouping.
        /// </summary>
        /// <param name="groupKey">The group for the object being dragged.</param>
        /// <param name="key">The object being dragged.</param>
        /// <param name="contentWidth">The pixel size of the object's width.</param>
        /// <param name="contentHeight">The pixel size of the object's height.</param>
        void StartDrag(object groupKey, object key, int contentWidth, int contentHeight);

        /// <summary>
        /// Stops the current drag-drop operation.
        /// </summary>
        void StopDrag();
    }
}