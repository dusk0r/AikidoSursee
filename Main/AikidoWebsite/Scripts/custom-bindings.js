ko.bindingHandlers.datetimepicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        $(element).datetimepicker({
            language: 'pt-BR'
        });

        ko.utils.registerEventHandler(element, "changeDate", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(event.date);
            }
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var picker = $(element).data('datetimepicker');

        if (picker) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            //alert("change: " + value);
            //picker.setLocalDate(value);
            picker.setDate(value);
        }
    }
};
