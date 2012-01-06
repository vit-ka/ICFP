function Memory() {
    this.store = [];
    this.loadToZeroArray = loadToZeroArray;
    this.getArrayLength = getArrayLength;
    this.getArrayElem = getArrayElem;
    this.setArrayElem = setArrayElem;
    this.createNewArray = createNewArray;
    this.abandonArray = abandonArray;
    this.copyArrayToZeroIndex = copyArrayToZeroIndex;
    this.findFreeIndex = findFreeIndex;
}

function loadToZeroArray(script) {
    this.store[0] = script;

    write_to_um_console("INFO: Memory array at index 0 has been loaded.");
}

function copyArrayToZeroIndex(scriptIndex) {
    if (scriptIndex == 0)
        return;

    var copiedArray = [];
    for (var i = 0; i < this.store[scriptIndex].length; ++i)
        copiedArray[i] = this.store[scriptIndex][i];
    this.store[0] = copiedArray;
}

function getArrayLength(index) {
    return this.store[index].length;
}

function getArrayElem(scriptIndex, elemIndex) {
    return this.store[scriptIndex][elemIndex];
}

function setArrayElem(scriptIndex, elemIndex, newValue) {
    this.store[scriptIndex][elemIndex] = newValue;
}

function createNewArray(capasity) {
    var newArray = [];
    for (var i = 0; i < capasity; ++i)
        newArray[i] = 0;

    var freeIndex = this.findFreeIndex();

    this.store[freeIndex] = newArray;

    return freeIndex;
}

function findFreeIndex() {
    for (var i = 0; i < this.store.length; ++i) {
        if (!this.store[i]) {
            console.log("Found free array at " + i);
            return i;
        }
    }

    return this.store.length;
}

function abandonArray(scriptIndex) {
    this.store[scriptIndex] = null;
}