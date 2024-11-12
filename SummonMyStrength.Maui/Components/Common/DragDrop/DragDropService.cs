using System.Collections.Generic;
using System.Linq;

namespace SummonMyStrength.Maui.Components.Common.DragDrop;

/// <inheritdoc />
internal class DragDropService : IDragDropService
{
    /// <summary>
    /// A registry of all dropsites that exist in the app which is used to prepare them to
    /// receive the content being dragged.
    /// </summary>
    internal static readonly IList<IDropsite> AllDropSites = new List<IDropsite>();

    /// <inheritdoc />
    public object ActiveGroupKey { get; private set; }

    /// <inheritdoc />
    public object ActiveKey { get; private set; }

    /// <inheritdoc />
    public int DragContentWidth { get; private set; }

    /// <inheritdoc />
    public int DragContentHeight { get; private set; }

    /// <inheritdoc />
    public IDropsite Dropsite { get; set; }

    /// <inheritdoc />
    public void StartDrag(object groupKey, object key, int contentWidth, int contentHeight)
    {
        System.Console.WriteLine($"Starting drag on element with dimentions: {contentWidth}x{contentHeight}");
        ActiveGroupKey = groupKey;
        ActiveKey = key;
        DragContentWidth = contentWidth;
        DragContentHeight = contentHeight;
        Dropsite = null;

        foreach (IDropsite site in AllDropSites.Where(x => x.IsValidDropSiteFor(groupKey, key)))
        {
            site.OnDragStart();
        }
    }

    /// <inheritdoc />
    public void StopDrag()
    {
        ActiveKey = null;
        Dropsite = null;
        DragContentWidth = 0;
        DragContentHeight = 0;

        foreach (var site in AllDropSites)
        {
            site.OnDragStop();
        }
    }
}
