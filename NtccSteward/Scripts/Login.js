/// <reference path="jquery-1.12.4.js" />
/// <reference path="jquery-1.12.4.intellisense.js" />

function initializeRequestAccount() {

    $("#requestAcct").validate({
        rules: {
            FirstName: {
                required: true
            }
                //,
            //LastName: {
            //    required: true
            //},
            //Email: {
            //    required: true,
            //    email: true,
            //},
            //Password: {
            //    required: true,
            //    minlength: 7,
            //},
            //password2: {
            //    required: true,
            //    equalTo: "#Password"
            //},
            //ChurchName: {
            //    required: true
            //},
            //PastorName: {
            //    required: true
            //},
            //City: {
            //    required: true
            //},
            //State: {
            //    required: true
            //}
        },

         // this method will prevent the form from being cleared automatically and let you process the fields before submitting to server.
        //  this is the standard way of handling submis
        submitHandler: function (form) {
            form.submit();
        },

        // called when error occurs
        highlight: function (element, errorClass) {
            // closest walks up the dom and finds the nearest element
            $(element).closest('.form-group').addClass('has-error');
        },

        // called when error is removed
        unhighlight: function (element, errorClass) {
            // closest walks up the dom and finds the nearest element
            $(element).closest('.form-group').removeClass('has-error');
        }
    });

}

function initializeLogin()
{
    $('#loginForm').validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true
            },
            ChurchId: {
                required: true
            }
        },
        highlight: function (element, errorClass) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element, errorClass) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        submitHandler: function (form) {
            form.submit();
            //alert(form.action);
            //$.ajax({
            //    url: form.action,
            //    type: form.method,
            //    data: $(form).serialize(),
            //    dataType: "text json",  // this was necessary to receive the Json() return result
            //    success: function (postResponse) {

            //        window.location.href = postResponse;
            //        // this is not working, need to figure out why
            //        //{"RedirectUrl":"/Account","ErrorMessage":"Login attempt failed, please try again.","Success":false}
            //        //alert(postResponse);
                         
            //    },
            //    fail: function (data) {
            //        alert(data);
            //    }
            //});
        }
    });
}

