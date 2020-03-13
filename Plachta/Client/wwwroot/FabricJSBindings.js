var FabricJSBindings = FabricJSBindings || {};
FabricJSBindings.create = (function (element) {
    FabricJSBindings.CanvasElRefs = FabricJSBindings.CanvasElRefs || {};
    FabricJSBindings.CanvasElRefs[element] = {
        canvas: new fabric.Canvas(element),
        elements: {}
    };
});

FabricJSBindings.addText = (function (element, id, objRef, properties) {
    console.log(properties);
    var text = new fabric.Text(properties.text, properties);

    text.on('selected', function () {
        objRef.invokeMethodAsync('Selected');
    });

    text.on('modified', function() {
        console.log(text);
        objRef.invokeMethodAsync('Update', JSON.stringify(text));
    });

    FabricJSBindings.CanvasElRefs[element].elements[id] = text;
    FabricJSBindings.CanvasElRefs[element].canvas.add(text);
});

FabricJSBindings.export = (function (element) {
    return FabricJSBindings.CanvasElRefs[element].canvas.toSVG();
});

FabricJSBindings.changeProperty = (function (element, id, data) {
    console.log(data);
    FabricJSBindings.CanvasElRefs[element].elements[id].set(data);
    FabricJSBindings.CanvasElRefs[element].canvas.renderAll();
});