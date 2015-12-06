$.validator.addMethod(
    "tracking",
    function(value, element) {
        var re = new RegExp("^\[a-zA-Z]{2}\\d{9}\[a-zA-Z]{2}$");
        return this.optional(element) || re.test(value);
    },
    "Изините, вам нужно ввести номер слежения вида 'AA123456789ZZ'. Другие номера слежения не отслеживаются сайтом belpost.by");

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
                    required: "Извините, вы забыли указать ваш адрес электронной почты.",
                    email: "Извините, похоже, вы где-то ошиблись."
                },
                tracking: {
                    required: "Извините, вы забыли указать номер слежения вашей посылки."
                },
                description: {
                    required: "Извините, вы забыли указать подсказку для посылки."
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

    	// select all desired input fields and attach tooltips to them
	    $('.hint').tooltipster({
		    theme: 'tooltipster-punk'
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