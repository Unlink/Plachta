var FabricJSBindings = FabricJSBindings || {};

document.addEventListener('keydown', (e) => {
    //console.log(e.keyCode);
    if (e.keyCode == 46) {
        for (var el in FabricJSBindings.CanvasElRefs) {
            var canvas = FabricJSBindings.CanvasElRefs[el].canvas;
            canvas.remove(canvas.getActiveObject());
        }
    }
    else if (e.keyCode == 38) { //up
        for (var el in FabricJSBindings.CanvasElRefs) {
            var canvas = FabricJSBindings.CanvasElRefs[el].canvas;
            canvas.bringToFront(canvas.getActiveObject());
        }
        e.preventDefault();
    }
    else if (e.keyCode == 40) { //down
        for (var el in FabricJSBindings.CanvasElRefs) {
            var canvas = FabricJSBindings.CanvasElRefs[el].canvas;
            canvas.sendToBack(canvas.getActiveObject());
        }
        e.preventDefault();
    }
});


FabricJSBindings.create = (function (element) {
    FabricJSBindings.CanvasElRefs = FabricJSBindings.CanvasElRefs || {};
    FabricJSBindings.CanvasElRefs[element] = {
        canvas: new fabric.Canvas(element),
        elements: {},
        dotNetRefs: {}
    };

    FabricJSBindings.CanvasElRefs[element].canvas.preserveObjectStacking = true;

    FabricJSBindings.CanvasElRefs[element].canvas.on('selection:created',
        (e) => {
            if (e.target.type === 'activeSelection') {
                FabricJSBindings.CanvasElRefs[element].canvas.discardActiveObject();
            }
        });

    FabricJSBindings.CanvasElRefs[element].canvas.on("selection:cleared",
        function (e) {
            if ("deselected" in e) {
                console.log(e);
                const canvas = FabricJSBindings.CanvasElRefs[element];
                for (var el of e.deselected) {
                    for (var elid in canvas.elements) {
                        if (el === canvas.elements[elid]) {
                            canvas.dotNetRefs[elid].invokeMethodAsync('Selected', false);
                        }
                    }
                }
            }
        });
});

FabricJSBindings.addText = (function (element, id, objRef, properties) {
    console.log(properties);
    var text = new fabric.Text(properties.text, properties);
    text.set({ id: id });

    console.log(text);

    text.on('selected', function () {
        objRef.invokeMethodAsync('Selected', true);
    });

    text.on('modified', function() {
        console.log(text);
        objRef.invokeMethodAsync('Update', JSON.stringify(text));
    });

    text.on('removed', function () {
        console.log("object removed");
        objRef.invokeMethodAsync('Remove');
    });

    FabricJSBindings.CanvasElRefs[element].elements[id] = text;
    FabricJSBindings.CanvasElRefs[element].dotNetRefs[id] = objRef;
    FabricJSBindings.CanvasElRefs[element].canvas.add(text);
});

FabricJSBindings.addImage = (function (element, id, objRef, properties) {
    console.log(properties);

    fabric.Image.fromURL(properties.image, function (oImg) {
        oImg.set(properties);

        oImg.on('selected', function () {
            objRef.invokeMethodAsync('Selected', true);
        });

        oImg.on('modified', function () {
            console.log(oImg);
            objRef.invokeMethodAsync('Update', JSON.stringify(oImg));
        });

        oImg.on('removed', function () {
            console.log("object removed");
            objRef.invokeMethodAsync('Remove');
        });

        FabricJSBindings.CanvasElRefs[element].elements[id] = oImg;
        FabricJSBindings.CanvasElRefs[element].dotNetRefs[id] = objRef;
        FabricJSBindings.CanvasElRefs[element].canvas.add(oImg);
    });
});

FabricJSBindings.export = (function (element) {
    return FabricJSBindings.CanvasElRefs[element].canvas.toSVG();
});

FabricJSBindings.changeProperty = (function (element, id, data) {
    console.log(data);
    FabricJSBindings.CanvasElRefs[element].elements[id].set(data);
    FabricJSBindings.CanvasElRefs[element].canvas.renderAll();
});

FabricJSBindings.dispose = (function (element) {
    console.log("Disposing canvas");
    FabricJSBindings.CanvasElRefs[element].canvas.dispose();
    delete FabricJSBindings.CanvasElRefs[element];
});

FabricJSBindings.resizeImage = (function (image, maxWidth, maxHeight) {

    var promise = new Promise(function(resolve, reject) {
        var img = new Image();
        img.onload = function() {
            var canvas = document.createElement('canvas'),
                ctx = canvas.getContext("2d"),
                oc = document.createElement('canvas'),
                octx = oc.getContext('2d');

            console.log(["original size", img.width, img.height]);

            canvas.width = Math.min(maxWidth, img.width);
            canvas.height = canvas.width * img.height / img.width;
            if (canvas.height > maxHeight) {
                canvas.height = maxHeight;
                canvas.width = canvas.height * img.width / img.height;
            }

            console.log(["new size", canvas.width, canvas.height]);

            //var cur = {
            //    width: Math.floor(img.width * 0.5),
            //    height: Math.floor(img.height * 0.5)
            //}

            //oc.width = cur.width;
            //oc.height = cur.height;

            //octx.drawImage(img, 0, 0, cur.width, cur.height);

            //while (cur.width * 0.5 > canvas.width) {
            //    cur = {
            //        width: Math.floor(cur.width * 0.5),
            //        height: Math.floor(cur.height * 0.5)
            //    };
            //    octx.drawImage(oc, 0, 0, cur.width * 2, cur.height * 2, 0, 0, cur.width, cur.height);
            //}

            //ctx.drawImage(oc, 0, 0, cur.width, cur.height, 0, 0, canvas.width, canvas.height);

            ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
            console.log("Conversion done");
            resolve({
                    width: canvas.width,
                    height: canvas.height,
                    data: canvas.toDataURL()
                }
            );
        }
        img.src = image;
    });

    return promise;
});