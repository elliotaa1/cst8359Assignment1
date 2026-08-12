"use strict";

const eventId = document.getElementById("eventId").value;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/eventHub")
    .withAutomaticReconnect()
    .build();

connection.on("AttendeeRegistered", function (count, attendeeName) {

    document.getElementById("attendeeCount").textContent = count;

    const attendeeList =
        document.getElementById("attendeeList");

    const attendee = document.createElement("p");

    attendee.textContent = attendeeName;

    attendeeList.appendChild(attendee);
});

connection.on("AttendeeUnregistered", function (count) {

    document.getElementById("attendeeCount").textContent = count;

    location.reload();
});

connection.on("OrganizerNotification", function (message) {

    const notification =
        document.getElementById("organizerNotification");

    if (notification) {
        notification.textContent = message;
        notification.style.display = "block";
    } else {
        alert(message);
    }
});

async function startConnection() {

    try {

        await connection.start();

        console.log("SignalR connected.");

        await connection.invoke(
            "JoinEventGroup",
            parseInt(eventId)
        );

    } catch (err) {

        console.error(err);

        setTimeout(startConnection, 5000);
    }
}

startConnection();