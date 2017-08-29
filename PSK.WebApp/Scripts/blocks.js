function AppendBlock(targetClass) {
    var dynamicListWrapper = $(".dynamic-list-wrapper." + targetClass);
    var dynamicListBody = $(".dynamic-list-body", dynamicListWrapper);
    var naujasIndeksas = parseInt(dynamicListBody.length, 10);

    var clonedBody = dynamicListBody.first().clone();

    $("input:text, input:hidden, input:radio, input:checkbox, input.k-textbox, textarea, select", clonedBody).not("input:button").val("");
    $("[value]", clonedBody).attr("value", "");
    $("input:radio, input:checkbox", clonedBody).prop("checked", false);
    
    $.each($("[data-default-value]", clonedBody), function (index, element) {
        $(element).attr("value", $(this).data("default-value")); 
    });
    
    $("[name$='.Index']", clonedBody).val(naujasIndeksas);

    var clonedBodyHtml = clonedBody[0].outerHTML;

    clonedBodyHtml = clonedBodyHtml.replace(/\_0\_/g, "_" + naujasIndeksas + "_");
    clonedBodyHtml = clonedBodyHtml.replace(/\[0\]\./g, "[" + naujasIndeksas + "].");

    dynamicListWrapper.append(clonedBodyHtml);
}

function RemoveBlock(e) {
    var dynamicListBody = $(e).closest(".dynamic-list-body");
    var dynamicListWrapper = dynamicListBody.closest(".dynamic-list-wrapper");
    if ($(".dynamic-list-body", dynamicListWrapper).length > 1)
        dynamicListBody.remove();
}