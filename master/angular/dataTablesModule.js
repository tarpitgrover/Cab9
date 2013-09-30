
var dataTablesModule = angular.module('DataTables', []);

dataTablesModule.directive('datatable', function () {
    return function(scope, element, attrs) {
        console.log('datatables directive in action');

        // apply DataTable options, use defaults if none specified by user
        var options = {};
        if (attrs.datatable.length > 0) {
            options = scope.$eval(attrs.datatable);
        } else {
        var options = {
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bInfo": false,
            "aoColumnDefs": [],
            "aaData": []
        };
        }

        //options["aoColumns"] = scope.$eval(attrs.aoColumns);
        options["aoColumnDefs"] = scope.$eval(attrs.aoColumnDefs);
        options["aaData"] = scope.myData;

        // Tell the dataTables plugin what columns to use
        // We can either derive them from the dom, or use setup from the controller           
        var explicitColumns = [];
        element.find('th').each(function(index, elem) {
            explicitColumns.push($(elem).text());
        });
        if (explicitColumns.length > 0) {
            options["aoColumns"] = explicitColumns;
        }
        //} else if (attrs.aoColumns) {
        //    options["aoColumns"] = scope.$eval(attrs.aoColumns);
        //}
            
        if (attrs.fnRowCallback) {
            options["fnRowCallback"] = scope.$eval(attrs.fnRowCallback);
        }

       
        // apply the plugin
        var dataTable = element.dataTable(options);

        var oTableTools = new TableTools(dataTable, {
            "aButtons": [
                "copy", "csv", "xls", "pdf"              
            ],
            "sSwfPath": "/includes/swf/copy_csv_xls_pdf.swf"
        });

        $('#tableTools').html(oTableTools.dom.container);
            
        // watch for any changes to our data, rebuild the DataTable
        scope.$watch('myData', function(value) {
            var val = value || null;
            if (val) {
                dataTable.fnClearTable();
                dataTable.fnAddData(scope.$eval('myData'));
            }
        });
    };
});