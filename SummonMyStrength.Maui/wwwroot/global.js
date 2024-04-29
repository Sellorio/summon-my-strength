// Drag-Drop

document.addEventListener("mousemove", e => wyDraggingUpdate(e));
document.addEventListener("mouseup", e => wyCancelDrag());

var wyDragPreparedElement = null;
var wyDragElement = null;
var wyDragWidth = null;
var wyDragHeight = null;

function wyGetDragWidth() {
    return wyDragWidth;
}

function wyGetDragHeight() {
    return wyDragHeight;
}

function wyStartDrag(element) {
    if (wyDragElement != null) {
        wyStopDrag();
    }

    wyDragWidth = element.children[0].clientWidth;
    wyDragHeight = element.children[0].clientHeight;
    element.children[0].classList.add("dragging");
    element.children[0].style.width = element.clientWidth + "px";
    wyDragElement = element;
    wyDragPreparedElement = null;
    element.children[1].click();
}

function wyStopDrag() {
    if (wyDragElement !== null) {
        wyDragElement.relativeMouseX = undefined;
        wyDragElement.relativeMouseY = undefined;
        wyDragElement.children[0].classList.remove("dragging");
        wyDragElement.children[0].style.left = "";
        wyDragElement.children[0].style.top = "";
        wyDragElement.children[0].style.width = "";
        wyDragElement.children[2].click();
        wyDragElement = null;
    }
}

function wyPrepareForDrag(element, event) {
    if (wyDragPreparedElement !== null || event.target instanceof HTMLInputElement) {
        return;
    }

    const rect = element.getBoundingClientRect();

    element.relativeMouseX = event.clientX - rect.left;
    element.relativeMouseY = event.clientY - rect.top;
    wyDragPreparedElement = element;
}

function wyCancelDrag() {
    if (wyDragElement !== null) {
        wyStopDrag();
    } else if (wyDragPreparedElement !== null) {
        wyDragPreparedElement.relativeMouseX = undefined;
        wyDragPreparedElement.relativeMouseY = undefined;
        wyDragPreparedElement = null;
    }
}

function wyDraggingUpdate(event) {
    if (wyDragElement !== null) {
        wyDragElement.children[0].style.left = event.clientX - wyDragElement.relativeMouseX + "px";
        wyDragElement.children[0].style.top = event.clientY - wyDragElement.relativeMouseY + "px";
    } else if (wyDragPreparedElement !== null) {
        const rect = wyDragPreparedElement.getBoundingClientRect();
        const deltaX = event.clientX - rect.left - wyDragPreparedElement.relativeMouseX;
        const deltaY = event.clientY - rect.top - wyDragPreparedElement.relativeMouseY;

        if (deltaX > 5 || deltaX < -5 || deltaY > 5 || deltaY < -5) {
            wyStartDrag(wyDragPreparedElement);
        }
    }
}
