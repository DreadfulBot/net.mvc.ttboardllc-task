function addTaskStatusChangeEventListener(parameters) {
    $('.task-status-selector').change(function(e) {
        var taskId = $(this).prop('id');
        var newStatusId = $(this).val();

        $.ajax({
            url: '/Task/UpdateStatus',
            data: { taskId : taskId, newStatusId : newStatusId },
            type: 'POST',
            success: function (result) {
                console.log("task " + taskId + " status changed to " + newStatusId);
                location.reload();
            },
            error: function (result) {
                console.log("task " + taskId + " status unable to change to " + newStatusId);
            }
        });
    });

    $('.checklist-status-selector').change(function (e) {
        var taskId = $(this).prop('id');
        var newStatusId = $(this).val();

        $.ajax({
            url: '/CheckList/UpdateStatus',
            data: { taskId: taskId, newStatusId: newStatusId },
            type: 'POST',
            success: function (result) {
                console.log("task " + taskId + " status changed to " + newStatusId);
                location.reload();
            },
            error: function (result) {
                console.log("task " + taskId + " status unable to change to " + newStatusId);
            }
        });
    });
}

function addCheckListChekEventListener(parameters) {
    $('.checklist-checkbox').change(function (e) {
        var checkListId = $(this).prop('id');
        var taskId = $(this).val();
        var isChecked = $(this).prop('checked');

        $.ajax({
            url: '/CheckList/ChangeState',
            data: { taskId: taskId, checkListId: checkListId, isChecked: isChecked },
            type: 'POST',
            success: function (result) {
                
                location.reload();
            },
            error: function (result) {
                
            }
        });
    });
}


// page load
$(document).ready(function () {
    addTaskStatusChangeEventListener();
    addCheckListChekEventListener();
});