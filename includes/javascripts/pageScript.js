$('.expandCollapseLeft').on('click', function () {
    $('#sidebarLeft').css("width", ($('#sidebarLeft').css("width") == "200px" ? "50px" : "200px"));
    $('#content').css("margin-left", ($('#content').css("margin-left") == "200px" ? "50px" : "200px"));
    $(this).find('span.glyphicon').removeClass($('#sidebarLeft').css("width") == "200px" ? "glyphicon-chevron-left" : "glyphicon-chevron-right");
    $(this).find('span.glyphicon').addClass($('#sidebarLeft').css("width") == "200px" ? "glyphicon-chevron-right" : "glyphicon-chevron-left");
    $('#sidebarLeft ul.mainMenu li span.menuIcon').css("border-bottom", $('#sidebarLeft').css("width") == "200px" ? "1px solid rgba(100, 100, 100, 0.5)" : "0");
    $('#sidebarLeft #credits').slideToggle();
    $('#sidebarLeft ul.mainMenu li.e9ine').slideToggle();
});

$('.expandCollapseRight').on('click', function () {
    $('#sidebarRight').css("width", ($('#sidebarRight').css("width") == "250px" ? "50px" : "250px"));
    $('#content').css("margin-right", ($('#content').css("margin-right") == "250px" ? "50px" : "250px"));
    $(this).find('span.glyphicon').removeClass($('#sidebarRight').css("width") == "250px" ? "glyphicon-chevron-right" : "glyphicon-chevron-left");
    $(this).find('span.glyphicon').addClass($('#sidebarRight').css("width") == "250px" ? "glyphicon-chevron-left" : "glyphicon-chevron-right");
    if ($('#sidebarRight').css("width") == "50px") {
        $('#sidebarRight').find('.section .body.popOut').slideUp();
        $('#sidebarRight').find('.section .body').removeClass("popOut");
        $('#sidebarRight').find('.section .body').slideDown();
    } else {
        $('#sidebarRight').find('.section .body').slideUp(function(){
            $('#sidebarRight').find('.section .body').addClass('popOut');
        });
    }
    $('#sidebarRight').find('.section .header .sectionName').slideToggle();
    $('#sidebarRight').find('.section').css("border-bottom", ($("#sidebarRight").css("width") == "250px" ? "1px solid rgba(100, 100, 100, 0.5)" : "5px solid rgba(100, 100, 100, 0.5)"));
});

$('.expandCollapsePhoneLeft').on('click', function () {
    $('#sidebarLeft').css("left", ($('#sidebarLeft').css("left") == "-200px" ? "0px" : "-200px"));
    $('#content').css("left", ($('#content').css("left") == "200px" ? "0px" : "200px"));
});

$('.expandCollapsePhoneRight').on('click', function () {
    $('#sidebarRight').css("right", ($('#sidebarRight').css("right") == "-250px" ? "0px" : "-250px"));
    $('#content').css("left", ($('#content').css("left") == "-250px" ? "0px" : "-250px"));
});



$('#userMenu, #userIcon').on('click', function () {
    $(this).parent('#loggedInUser').find('#userMenuList').slideToggle(100);
});

$('#sidebarRight .section .sectionIcon').on('click', function () {
    $('#sidebarRight').css('overflow', 'visible');
    $('#sidebarRight .section .body.popOut').hide()
    $(this).parent('.header').parent('.section').find('.popOut').toggle();
    var height = $(this).parent('.header').parent('.section').find('.popOut').height();
    var windowHeight = $(window).height();
    var top = $(this).parent('.header').parent('.section').find('.popOut').offset().top - $(window).scrollTop();
    var abc = height + top;
    if ((height + top) > windowHeight) {
        $(this).parent('.header').parent('.section').find('.popOut').css('margin-top',-1*((top-windowHeight)+height+65)+'px');
    }
});

$('#sidebarRight .section .body .sectionBodyClose').on('click', function () {
    $('#sidebarRight .section .body.popOut').hide();
    $('#sidebarRight').css('overflow-y', 'scroll');
});

$('#userMenuList').mouseleave(function () {
    $(this).slideUp(200);
});

$(function () {
    $("[data-toggle='tooltip']").tooltip();
});

$(window).resize(function () {
        $('#sidebarRight .section .body').removeClass('popOut').show();
        $('#sidebarRight').find('.section .header .sectionName').show();
        $('#sidebarRight').css('overflow-y', 'scroll');
        if ($(this).width() > 1023) {
            $('#sidebarLeft').css('width', '200px').css('left', '0');
            $('#sidebarRight').css('width', '250px').css('right', '0');
            $('#content').css('margin-left', '200px').css('margin-right', '250px').css('left', '0');
        }
        else {
            $('#sidebarLeft').css('left', '-200px');
            $('#sidebarRight').css('right', '-250px');
            $('#content').css('margin-left', '0px').css('margin-right', '0px').css('left', '0');
        }
});

function startclock()
{
    var thetime=new Date();

    var nhours=thetime.getHours();
    var nmins=thetime.getMinutes();
    var nsecn=thetime.getSeconds();
    var nmonth=thetime.getMonth()+1;
    var ntoday=thetime.getDate();
    var nyear=thetime.getYear();

    if (ntoday < 10)
        ntoday = "0" + ntoday;

    if (nmonth < 10)
        nmonth = "0" + nmonth;

    if (nhours < 10)
        nhours = "0" + nhours;

    if (nsecn<10)
        nsecn="0"+nsecn;

    if (nmins<10)
        nmins="0"+nmins;

    if (nyear<=99)
        nyear= "19"+nyear;

    if ((nyear>99) && (nyear<2000))
        nyear+=1900;

    document.getElementById("dateTime").innerHTML = ntoday + "/" + nmonth + "/" + nyear + "<br /><span class='time'>" + nhours + ":" + nmins + ":" + nsecn + "</span>";

    setTimeout('startclock()',1000);
} 

