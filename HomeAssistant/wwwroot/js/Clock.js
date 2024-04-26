let hour, min;

let clock = document.getElementById("clock");

if (clock !== null) {
    let arr = clock.textContent.split(":");
    hour = parseInt(arr[0]);
    min = parseInt(arr[1]);

    Timer(hour, min);
}

function Timer(hour, min) {


    if (min === 59) {
        hour++;
        min = 0;
    }
    else {
        min++;
    }

    clock.innerHTML = `${hour.toLocaleString('en-US', { minimumIntegerDigits: 2, useGrouping: false })}:${min.toLocaleString('en-US', { minimumIntegerDigits: 2, useGrouping: false })}`;

    setTimeout(function () {
        if (hour===23 && min===59) {
            location.reload();
            return;
        }
        Timer(hour, min);
    }, 60000);
}