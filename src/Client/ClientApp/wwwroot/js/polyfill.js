// .replaceAll will be available starting on Chrome 85
// Текущая верcия браузера Спутник - 5.6.6282.0 посторена на более старой версии Chrome.
// https://stackoverflow.com/questions/62825358/javascript-replaceall-is-not-a-function-type-error
if (typeof String.prototype.replaceAll === "undefined") {
    String.prototype.replaceAll = function(match, replace) {
       return this.replace(new RegExp(match, 'g'), () => replace);
    }
}
