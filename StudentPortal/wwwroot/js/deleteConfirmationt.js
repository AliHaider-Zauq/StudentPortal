function showDeleteConfirmation(userId) {
    document.getElementById("confirmDelete-" + userId).style.display = "block";
}

function hideDeleteConfirmation(userId) {
    document.getElementById("confirmDelete-" + userId).style.display = "none";
}
