// wwwroot/js/print.js
window.printSaleReceipt = function (saleNumber) {
    const printContent = document.getElementById('print-receipt-' + saleNumber);
    if (!printContent) {
        console.error('Print content not found');
        alert('Print content not found');
        return false;
    }

    const windowPrint = window.open('', '_blank', 'width=800,height=600,scrollbars=yes');

    if (!windowPrint) {
        alert('Pop-up blocked. Please allow pop-ups for this site.');
        return false;
    }

    windowPrint.document.write('<html><head><title>Sale Receipt - ' + saleNumber + '</title>');
    windowPrint.document.write('<style>');
    windowPrint.document.write(`
        body { 
            font-family: Arial, sans-serif; 
            margin: 20px;
            font-size: 12px;
        }
        .receipt-header { 
            text-align: center; 
            border-bottom: 2px solid #000; 
            padding-bottom: 15px;
            margin-bottom: 20px;
        }
        .receipt-header h2 { 
            margin: 0; 
            font-size: 24px;
        }
        .receipt-header p { 
            margin: 5px 0; 
            color: #666;
        }
        .receipt-info { 
            margin-bottom: 20px; 
        }
        .receipt-info table { 
            width: 100%; 
        }
        .receipt-info td { 
            padding: 5px; 
        }
        .receipt-info td:first-child { 
            font-weight: bold; 
            width: 150px;
        }
        table.items { 
            width: 100%; 
            border-collapse: collapse; 
            margin-bottom: 20px;
        }
        table.items th, table.items td { 
            border: 1px solid #ddd; 
            padding: 8px; 
            text-align: left;
        }
        table.items th { 
            background-color: #f2f2f2; 
            font-weight: bold;
        }
        table.items td.text-end { 
            text-align: right;
        }
        table.items tfoot td { 
            font-weight: bold;
            background-color: #f9f9f9;
        }
        .signature-section { 
            margin-top: 50px; 
            page-break-inside: avoid;
        }
        .signature-line { 
            border-top: 1px solid #000; 
            width: 250px; 
            margin-top: 60px;
            text-align: center;
            padding-top: 5px;
        }
        .receipt-footer { 
            text-align: center; 
            margin-top: 40px; 
            border-top: 2px solid #000; 
            padding-top: 15px;
            color: #666;
        }
        @media print {
            body { margin: 0; }
            .no-print { display: none; }
        }
    `);
    windowPrint.document.write('</style></head><body>');
    windowPrint.document.write(printContent.innerHTML);
    windowPrint.document.write('</body></html>');
    windowPrint.document.close();

    windowPrint.onload = function () {
        windowPrint.focus();
        setTimeout(function () {
            windowPrint.print();
        }, 250);
    };

    return false;
};