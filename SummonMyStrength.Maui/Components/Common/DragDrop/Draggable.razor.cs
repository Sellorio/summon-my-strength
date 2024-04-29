using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace SummonMyStrength.Maui.Components.Common.DragDrop
{
    /// <summary>
    /// A component used to allow content to be dragged from one place to another.
    /// </summary>
    /// <typeparam name="T">The type of the object being moved.</typeparam>
    public partial class Draggable<T>
    {
        private DragDropsite<T> LocalDropsite;
        private ElementReference ChildRef;

        /// <summary>
        /// Gets or sets a group used to scope drag-drop operations to control possible dropsites.
        /// </summary>
        [Parameter]
        public object GroupKey { get; set; }

        /// <summary>
        /// Gets or sets the object being moved during a drag operation.
        /// </summary>
        [Parameter]
        public T Key { get; set; }

        /// <summary>
        /// Gets or sets the content to be displayed for the draggable object.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets an event handler called when a drag-drop operation is completed. Use this to remove
        /// the object from a list if applicable.
        /// </summary>
        [Parameter]
        public EventCallback OnDragCompleted { get; set; }

        /// <summary>
        /// Gets or sets an event handler called when an object is dropped on this element (inserted before).
        /// </summary>
        [Parameter]
        public EventCallback<T> OnInsertDrop { get; set; }

        /// <summary>
        /// Gets or sets an event handler called when the wrapped content has started being dragged.
        /// </summary>
        [Parameter]
        public EventCallback OnDragStart { get; set; }

        /// <summary>
        /// Gets or sets an event handler called when the wrapped content is no longer being dragged.
        /// </summary>
        [Parameter]
        public EventCallback OnDragStop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not drag drop functionality is disabled.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to disable the drag dropsite that is included by
        /// default as part of the draggable wrapper.
        /// </summary>
        [Parameter]
        public bool DisableDropsite { get; set; }

        private async Task StartDragAsync()
        {
            int width = await JS.InvokeAsync<int>("wyGetDragWidth", ChildRef);
            int height = await JS.InvokeAsync<int>("wyGetDragHeight", ChildRef);
            System.Console.WriteLine($"Starting drag on element with dimentions: {width}x{height}");
            DragDropService.StartDrag(GroupKey, Key, width, height);
            await OnDragStart.InvokeAsync();
        }

        private async Task EndDragAsync()
        {
            if (ReferenceEquals(DragDropService.ActiveKey, Key))
            {
                if (DragDropService.Dropsite != null)
                {
                    var isLocalDrop = DragDropService.Dropsite == LocalDropsite;

                    if (!isLocalDrop)
                    {
                        await OnDragCompleted.InvokeAsync();
                        await DragDropService.Dropsite.OnDropAsync(DragDropService.ActiveKey);
                    }
                }

                DragDropService.StopDrag();
            }

            await OnDragStop.InvokeAsync();
        }

        private async Task OnDropAsync(T droppedObject)
        {
            if (!ReferenceEquals(droppedObject, Key))
            {
                await OnInsertDrop.InvokeAsync(droppedObject);
            }
        }
    }
}
