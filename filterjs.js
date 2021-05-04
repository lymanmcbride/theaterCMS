function greaterThanLessThanClick() {
    if (document.getElementById("greater_less_input").value == "less") {
        document.getElementById("GreaterThan_LessThan").classList.remove('fa-chevron-left');
        document.getElementById("GreaterThan_LessThan").classList.add('fa-chevron-right');
        document.getElementById("greater_less_input").value = "greater"
    }
    else {
        document.getElementById("GreaterThan_LessThan").classList.remove('fa-chevron-right');
        document.getElementById("GreaterThan_LessThan").classList.add('fa-chevron-left');
        document.getElementById("greater_less_input").value = "less"
    }
}

//add greater than or less than sign to page. Default is less-than on view.
function gTLTLoad(value) {
    if (value == "less") {
        document.getElementById("GreaterThan_LessThan").classList.add('fa-chevron-left');
    }
    else if (value == "greater") {
        document.getElementById("GreaterThan_LessThan").classList.add('fa-chevron-right');
    }
}

//call above function onload
if (document.URL.includes("/Rent/Rentals")) {
    window.onload = gTLTLoad(document.getElementById("greater_less_input").value);
}