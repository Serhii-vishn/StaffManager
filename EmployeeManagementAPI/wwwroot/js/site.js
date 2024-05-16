function toggleDropdown() {
    document.getElementById("dropdown-content").classList.toggle("show");
}

window.onclick = function (event) {
    if (!event.target.closest('.dropdown')) {
        var dropdowns = document.getElementsByClassName("dropdown-content");
        for (var i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}

document.querySelector('.dropdown-form').addEventListener('submit', function () {
    var dropdown = document.getElementById("dropdown-content");
    if (dropdown.classList.contains('show')) {
        dropdown.classList.remove('show');
    }
});

var startRange = document.getElementById('startYearRange');
var endRange = document.getElementById('endYearRange');
var startDisplay = document.getElementById('startYearDisplay');
var endDisplay = document.getElementById('endYearDisplay');

function updateDisplay(type) {
    var range;
    var display;

    if (type === 'start') {
        range = startRange;
        display = startDisplay;
    } else {
        range = endRange;
        display = endDisplay;
    }

    display.textContent = range.value;
}
