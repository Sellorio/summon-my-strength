using Microsoft.AspNetCore.Components;

namespace SummonMyStrength.Maui.Components.Common.DragDrop;

/// <summary>
/// A component used as a drop target for drag-drop operations.
/// </summary>
/// <typeparam name="T">The type of the object being drag-dropped.</typeparam>
public partial class DragDropsite<T> : IDropsite, IDisposable
{
    private int _width;
    private int _height;
    private bool _isActive;

    /// <summary>
    /// Gets or sets the injected <see cref="IDragDropService"/> instance.
    /// </summary>
    [Inject]
    public IDragDropService DragDropService { get; set; }

    /// <summary>
    /// Gets or sets a group key used to avoid drag drop from unexpected sources.
    /// </summary>
    [Parameter]
    public object GroupKey { get; set; }

    /// <summary>
    /// Gets or sets the handler called when an object is dropped on this site.
    /// </summary>
    [Parameter]
    public EventCallback<T> OnDrop { get; set; }

    /// <summary>
    /// Gets or sets a wrapper for the active dropsite preview to account for any special layout concerns - such as if the dropsite
    /// is in a MudGrid.
    /// </summary>
    [Parameter]
    public RenderFragment<RenderFragment> ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        DragDrop.DragDropService.AllDropSites.Add(this);
    }

    /// <inheritdoc />
    public async Task OnDropAsync(object droppedObject)
    {
        await OnDrop.InvokeAsync((T)droppedObject);
    }

    /// <inheritdoc />
    public void OnLeave()
    {
        _isActive = false;
        StateHasChanged();
    }

    /// <inheritdoc />
    public void OnDragStart()
    {
        _width = DragDropService.DragContentWidth;
        _height = DragDropService.DragContentHeight;
        StateHasChanged();
    }

    /// <inheritdoc />
    public void OnDragStop()
    {
        _width = 0;
        _height = 0;
        _isActive = false;
        StateHasChanged();
    }

    /// <inheritdoc />
    public bool IsValidDropSiteFor(object groupKey, object key)
    {
        bool isValid =
            key != null &&
            key is T &&
            ((GroupKey == null && groupKey == null) ||
                Equals(GroupKey, groupKey));

        Console.WriteLine("Is Valid Drop Site: " + isValid);

        return isValid;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DragDrop.DragDropService.AllDropSites.Remove(this);
    }

    private void DragEntering()
    {
        if (DragDropService.ActiveKey != null && DragDropService.ActiveKey is T)
        {
            if (DragDropService.Dropsite != null)
            {
                DragDropService.Dropsite.OnLeave();
            }

            DragDropService.Dropsite = this;
            _isActive = true;
        }
    }

    private void DragLeaving()
    {
        if (DragDropService.Dropsite == this)
        {
            DragDropService.Dropsite = null;
            _isActive = false;
        }
    }
}
