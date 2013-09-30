jQuery(document).ready(function() {
	
});

$("#expandSidebar").click(function () {
	$("#sidebar").addClass("expandedSidebar");
    $(this).hide();
	$("#contractSidebar").show();      
});

$("#contractSidebar").click(function () {
	$("#sidebar").removeClass("expandedSidebar");
    $(this).hide();
	$("#expandSidebar").show();      
});