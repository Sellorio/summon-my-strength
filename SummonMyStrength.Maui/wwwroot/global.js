async function sandboxedEval(script, ...args) {
    const iframeContent = `
<html>
    <head></head>
    <body>
        <script>
            window.addEventListener("message", event => {
                const functionWhichOutputsTheFunction = new Function("return " + event.data.code);
                const theFunction = functionWhichOutputsTheFunction();
                const output = theFunction(...event.data.args);
                window.parent.postMessage(output, "*");
            });
        </script>
    </body>
</html>
`;
    let promiseResolve;

    const promise = new Promise((resolve) => {
        promiseResolve = resolve;
    });

    const onMessage = event => {
        if (event.source === iframe.contentWindow) {
            promiseResolve(event.data);
        }
    };

    const iframe = document.createElement('iframe');
    iframe.src = "data:text/html;charset=utf-8," + iframeContent;
    iframe.style.cssText = "position: absolute; visibility: hidden; pointer-events; none";
    iframe.onload = () => {
        iframe.contentWindow.postMessage({ code: script, args: args }, "*");
    };

    let output;

    window.addEventListener("message", onMessage);

    try {
        document.body.appendChild(iframe);

        try {
            output = await promise;
        }
        finally {
            iframe.remove();
        }
    }
    finally {
        window.removeEventListener("message", onMessage);
    }

    return output;
}

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
