var lng = { "S": [[3], [1, "S"], [1, "S", 2, "S"]] };
lng["S"] = lng["S"].reverse();
var rules = ["S"];
var input = [1 ,1,3,2,1,3,2,3];
//var input = [1,1,2];
//var input = [3,1];
var start = rules[0];
var globalInputIndex = 0;
var Node = require("tree-node");

function main() {

    //console.log(expandRule(start, 0));
    var root = new Node();
    root.data({value: start});
    var status = expandRule(start, 0, root);
    if (status.val == input.length) {
        console.log("GUD");
    }
    else {
        console.log("BED");
    }

}

function expandRule(rule, inputIndex, root) {
    var optionIndex = 0;
    while (optionIndex < lng[rule].length) {

        var option = lng[rule][optionIndex];
        var success = expandOption(option, 0, inputIndex, root);
        if (success.val > 0) {
            root.appendChild(success.node);
            return success;
        }
        optionIndex += 1;

    }
    return {val : -1, node: null};
}

function expandOption(option, optionIndex, inputIndex, root) {
    while (optionIndex < option.length) {

        if (isNaN(option[optionIndex])) {

            //is a prod
            var success = expandRule(option[optionIndex], inputIndex);
            if (success.val > 0) {
                inputIndex = success.val;
                optionIndex += 1;

            }
            else {
                //console.error("Bad input");
                return {val : -1, node: null};
            }
        } else {
            //is terminal
            console.log(input[inputIndex] + " - " + option[optionIndex]);
            if (input[inputIndex] == option[optionIndex]) {

                inputIndex += 1;
                optionIndex += 1;
                
            }
            else {
                //console.error("Unexpected character");
                return {val : -1, node: null};
            }
        }
    }
    return {val: inputIndex, node: null};
}

// function main() {
//     var inputIndex = 0;
//     var ruleIndex = 0;
//     while (ruleIndex < rules.length) {
//         var success = expandRule(rules[ruleIndex], inputIndex);
//         if (success) {
//             return true;
//         }
//         ruleIndex += 1;
//     }
//     return false;
// }
// //gets key of rule and input index
// function expandRule(rule, inputIndex) {
//     var optionIndex = 0;
//     while (optionIndex < lng[rule].length) {
//         //console.log(lng[rule][optionIndex]);
//         var option = lng[rule][optionIndex];
//         var success = expandOption
//         optionIndex += 1;
//     }
// }

main();