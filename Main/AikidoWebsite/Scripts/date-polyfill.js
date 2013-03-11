﻿// Generated by CoffeeScript 1.3.3

/*
HTML5 Date polyfill | Jonathan Stipe | https://github.com/jonstipe/date-polyfill
*/


(function () {

    (function ($) {
        $.fn.inputDate = function () {
            var decrement, increment, makeDateDisplayString, makeDateString, readDate, stepNormalize;
            readDate = function (d_str) {
                var dateObj, dayPart, matchData, monthPart, yearPart;
                if (/^\d{4,}-\d\d-\d\d$/.test(d_str)) {
                    matchData = /^(\d+)-(\d+)-(\d+)$/.exec(d_str);
                    yearPart = parseInt(matchData[1], 10);
                    monthPart = parseInt(matchData[2], 10);
                    dayPart = parseInt(matchData[3], 10);
                    dateObj = new Date(yearPart, monthPart - 1, dayPart);
                    return dateObj;
                } else {
                    throw "Invalid date string: " + d_str;
                }
            };
            makeDateString = function (date_obj) {
                var d_arr;
                d_arr = [date_obj.getFullYear().toString()];
                d_arr.push('-');
                if (date_obj.getMonth() < 9) {
                    d_arr.push('0');
                }
                d_arr.push((date_obj.getMonth() + 1).toString());
                d_arr.push('-');
                if (date_obj.getDate() < 10) {
                    d_arr.push('0');
                }
                d_arr.push(date_obj.getDate().toString());
                return d_arr.join('');
            };
            makeDateDisplayString = function (date_obj, elem) {
                var $elem, date_arr, day_names, month_names;
                $elem = $(elem);
                day_names = $elem.datepicker("option", "dayNames");
                month_names = $elem.datepicker("option", "monthNames");
                date_arr = [day_names[date_obj.getDay()]];
                date_arr.push(', ');
                date_arr.push(month_names[date_obj.getMonth()]);
                date_arr.push(' ');
                date_arr.push(date_obj.getDate().toString());
                date_arr.push(', ');
                date_arr.push(date_obj.getFullYear().toString());
                return date_arr.join('');
            };
            increment = function (hiddenField, dateBtn, calendarDiv) {
                var $hiddenField, max, step, value;
                $hiddenField = $(hiddenField);
                value = readDate($hiddenField.val());
                step = $hiddenField.data("step");
                max = $hiddenField.data("max");
                if (!(step != null) || step === 'any') {
                    value.setDate(value.getDate() + 1);
                } else {
                    value.setDate(value.getDate() + step);
                }
                if ((max != null) && value > max) {
                    value.setTime(max.getTime());
                }
                value = stepNormalize(value, hiddenField);
                $hiddenField.val(makeDateString(value)).change();
                $(dateBtn).text(makeDateDisplayString(value, calendarDiv));
                $(calendarDiv).datepicker("setDate", value);
                return null;
            };
            decrement = function (hiddenField, dateBtn, calendarDiv) {
                var $hiddenField, min, step, value;
                $hiddenField = $(hiddenField);
                value = readDate($hiddenField.val());
                step = $hiddenField.data("step");
                min = $hiddenField.data("min");
                if (!(step != null) || step === 'any') {
                    value.setDate(value.getDate() - 1);
                } else {
                    value.setDate(value.getDate() - step);
                }
                if ((min != null) && value < min) {
                    value.setTime(min.getTime());
                }
                value = stepNormalize(value, hiddenField);
                $hiddenField.val(makeDateString(value)).change();
                $(dateBtn).text(makeDateDisplayString(value, calendarDiv));
                $(calendarDiv).datepicker("setDate", value);
                return null;
            };
            stepNormalize = function (inDate, hiddenField) {
                var $hiddenField, kNum, max, min, minNum, raisedStep, step, stepDiff, stepDiff2;
                $hiddenField = $(hiddenField);
                step = $hiddenField.data("step");
                min = $hiddenField.data("min");
                max = $hiddenField.data("max");
                if ((step != null) && step !== 'any') {
                    kNum = inDate.getTime();
                    raisedStep = step * 86400000;
                    if (min == null) {
                        min = new Date(1970, 0, 1);
                    }
                    minNum = min.getTime();
                    stepDiff = (kNum - minNum) % raisedStep;
                    stepDiff2 = raisedStep - stepDiff;
                    if (stepDiff === 0) {
                        return inDate;
                    } else {
                        if (stepDiff > stepDiff2) {
                            return new Date(inDate.getTime() + stepDiff2);
                        } else {
                            return new Date(inDate.getTime() - stepDiff);
                        }
                    }
                } else {
                    return inDate;
                }
            };
            $(this).filter('input[type="date"]').each(function () {
                var $calendarContainer, $calendarDiv, $dateBtn, $hiddenField, $this, calendarContainer, calendarDiv, className, closeFunc, dateBtn, hiddenField, max, min, step, style, value;
                $this = $(this);
                value = $this.attr('value');
                min = $this.attr('min');
                max = $this.attr('max');
                step = $this.attr('step');
                className = $this.attr('class');
                style = $this.attr('style');
                if ((value != null) && /^\d{4,}-\d\d-\d\d$/.test(value)) {
                    value = readDate(value);
                } else {
                    value = new Date();
                }
                if (min != null) {
                    min = readDate(min);
                    if (value < min) {
                        value.setTime(min.getTime());
                    }
                }
                if (max != null) {
                    max = readDate(max);
                    if (value > max) {
                        value.setTime(max.getTime());
                    }
                }
                if ((step != null) && step !== 'any') {
                    step = parseInt(step, 10);
                }
                hiddenField = document.createElement('input');
                $hiddenField = $(hiddenField);
                $hiddenField.attr({
                    type: "hidden",
                    name: $this.attr('name'),
                    value: makeDateString(value)
                });
                $hiddenField.data({
                    min: min,
                    max: max,
                    step: step
                });
                value = stepNormalize(value, hiddenField);
                $hiddenField.attr('value', makeDateString(value));
                calendarContainer = document.createElement('span');
                $calendarContainer = $(calendarContainer);
                if (className != null) {
                    $calendarContainer.attr('class', className);
                }
                if (style != null) {
                    $calendarContainer.attr('style', style);
                }
                calendarDiv = document.createElement('div');
                $calendarDiv = $(calendarDiv);
                $calendarDiv.css({
                    display: 'none',
                    position: 'absolute'
                });
                dateBtn = document.createElement('button');
                $dateBtn = $(dateBtn);
                $dateBtn.addClass('date-datepicker-button');
                $this.replaceWith(hiddenField);
                $calendarContainer.insertAfter(hiddenField);
                $dateBtn.appendTo(calendarContainer);
                $calendarDiv.appendTo(calendarContainer);
                $calendarDiv.datepicker({
                    dateFormat: 'MM dd, yy',
                    showButtonPanel: true,
                    beforeShowDay: function (dateObj) {
                        var dateDays, minDays;
                        if (!(step != null) || step === 'any') {
                            return [true, ''];
                        } else {
                            if (min == null) {
                                min = new Date(1970, 0, 1);
                            }
                            dateDays = Math.floor(dateObj.getTime() / 86400000);
                            minDays = Math.floor(min.getTime() / 86400000);
                            return [(dateDays - minDays) % step === 0, ''];
                        }
                    }
                });
                $dateBtn.text(makeDateDisplayString(value, calendarDiv));
                if (min != null) {
                    $calendarDiv.datepicker("option", "minDate", min);
                }
                if (max != null) {
                    $calendarDiv.datepicker("option", "maxDate", max);
                }
                if (Modernizr.csstransitions) {
                    calendarDiv.className = "date-calendar-dialog date-closed";
                    $dateBtn.click(function (event) {
                        $calendarDiv.off('transitionend oTransitionEnd webkitTransitionEnd MSTransitionEnd');
                        calendarDiv.style.display = 'block';
                        calendarDiv.className = "date-calendar-dialog date-open";
                        event.preventDefault();
                        return false;
                    });
                    closeFunc = function (event) {
                        var transitionend_function;
                        if (calendarDiv.className === "date-calendar-dialog date-open") {
                            transitionend_function = function (event, ui) {
                                calendarDiv.style.display = 'none';
                                $calendarDiv.off("transitionend oTransitionEnd webkitTransitionEnd MSTransitionEnd", transitionend_function);
                                return null;
                            };
                            $calendarDiv.on("transitionend oTransitionEnd webkitTransitionEnd MSTransitionEnd", transitionend_function);
                            calendarDiv.className = "date-calendar-dialog date-closed";
                        }
                        if (event != null) {
                            event.preventDefault();
                        }
                        return null;
                    };
                } else {
                    $dateBtn.click(function (event) {
                        $calendarDiv.fadeIn('fast');
                        event.preventDefault();
                        return false;
                    });
                    closeFunc = function (event) {
                        $calendarDiv.fadeOut('fast');
                        if (event != null) {
                            event.preventDefault();
                        }
                        return null;
                    };
                }
                $calendarDiv.mouseleave(closeFunc);
                $calendarDiv.datepicker("option", "onSelect", function (dateText, inst) {
                    var dateObj;
                    dateObj = $.datepicker.parseDate('MM dd, yy', dateText);
                    $hiddenField.val(makeDateString(dateObj)).change();
                    $dateBtn.text(makeDateDisplayString(dateObj, calendarDiv));
                    closeFunc();
                    return null;
                });
                $calendarDiv.datepicker("setDate", value);
                $dateBtn.on({
                    DOMMouseScroll: function (event) {
                        if (event.originalEvent.detail < 0) {
                            increment(hiddenField, dateBtn, calendarDiv);
                        } else {
                            decrement(hiddenField, dateBtn, calendarDiv);
                        }
                        event.preventDefault();
                        return null;
                    },
                    mousewheel: function (event) {
                        if (event.originalEvent.wheelDelta > 0) {
                            increment(hiddenField, dateBtn, calendarDiv);
                        } else {
                            decrement(hiddenField, dateBtn, calendarDiv);
                        }
                        event.preventDefault();
                        return null;
                    },
                    keypress: function (event) {
                        if (event.keyCode === 38) {
                            increment(hiddenField, dateBtn, calendarDiv);
                            event.preventDefault();
                        } else if (event.keyCode === 40) {
                            decrement(hiddenField, dateBtn, calendarDiv);
                            event.preventDefault();
                        }
                        return null;
                    }
                });
                return null;
            });
            return this;
        };
        $(function () {
            if (!Modernizr.inputtypes.date) {
                $('input[type="date"]').inputDate();
            }
            return null;
        });
        return null;
    })(jQuery);

}).call(this);
