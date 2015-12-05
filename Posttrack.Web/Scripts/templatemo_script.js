$.validator.addMethod(
    "tracking",
    function(value, element) {
        return true;
        // Validation is disabled because china started using tracking numbers like 44596888337 and WDG30865967CN
        var re = new RegExp("^\\D{2}\\d{9}\\D{2}$");
        return this.optional(element) || re.test(value);
    },
    "Пожалуйста, введите корректный номер отправления (пример: RC464177591CN)");

jQuery(function($) {
    $(window).load(function() { // makes sure the whole site is loaded
        $("#status").fadeOut(); // will first fade out the loading animation
        $("#preloader").delay(350).fadeOut("slow"); // will fade out the white DIV that covers the website.
        $("#main-wrapper").delay(350).css({ 'overflow': "visible" });
    });
    $("#email").keyup(function() {
        $(".alert-success").hide();
    });
    $("#tracking").keyup(function() {
        $(".alert-success").hide();
    });
    $("#description").keyup(function() {
        $(".alert-success").hide();
    });

    $(document).ready(function() {
        // backstretch for background image
        var defaultImgSrc = $("img.main-img").attr("src");
        $.backstretch(defaultImgSrc, { speed: 500 });
        var emailFromCookie = readCookie("email");
        if (emailFromCookie != null) {
            $("#email").val(emailFromCookie);
            $("#email").addClass("valid");
        }
        if ($("#email").val() != "") {
            $("#tracking").focus();
        } else {
            $("#email").focus();
        }

        $("#tracking-form").validate(
        {
            rules: {
                email: {
                    email: true,
                    required: true
                },
                tracking: {
                    required: true
                },
                description: {
                    required: true
                }
            },
            messages: {
                email: {
                    required: "Извините, вы забыли указать ваш адрес электронной почты. Без него мы не будем знать, куда высылать обновления статуса посылки.",
                    email: "Извините, вы уверены, что правильно введи адрес электронной почты? Похоже, вы где-то ошиблись."
                },
                tracking: {
                    required: "Извините, вы забыли указать номер вашего почтового отправления, без него мы не сможем отслеживать вашу посылку."
                },
                description: {
                    required: ""
                }
            },
            highlight: function(element) {
                $(element).removeClass("valid").addClass("error");
            },
            success: function(element) {
                $(element).removeClass("error").addClass("valid");
            }
        });

        $("#tracking").rules("add", { tracking: "" });
        $(".btn-primary").click(function() {
            if ($("#tracking-form").valid()) {
                $(this).prop("disabled", true);
                $.ajax({
                    url: "/Tracking",
                    async: false,
                    type: "POST",
                    dataType: "json",
                    data: {
                    	email: $.trim($("#email").val()),
                    	tracking: $.trim($("#tracking").val()),
                    	description: $.trim($("#description").val())
                    },
                    success: function(data) {
                        if (data.Errors && data.Errors.length > 0) {
                            var validator = $("#tracking-form").validate();
                            validator.showErrors({ "tracking": data.Errors[0].Value[0] });
                        } else {
                        	createCookie("email", $("#email").val(), 365);
	                        $("#email-sent").text($("#email").val());
                            $(".alert-success").fadeIn();
                        }
                    }
                });
                $(this).prop("disabled", false);
            }
        });
    });
});

function createCookie(name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = escape(name) + "=" + escape(value) + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = escape(name) + "=";
    var ca = document.cookie.split(";");
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === " ") c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return unescape(c.substring(nameEQ.length, c.length));
    }
    return null;
}