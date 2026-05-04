mergeInto(LibraryManager.library, {
  SetCursorPointer: function () {
    console.log("JS POINTER");
    Module.canvas.style.cursor = "pointer";
  },
  SetCursorDefault: function () {
    console.log("JS DEFAULT");
    Module.canvas.style.cursor = "default";
  }
});