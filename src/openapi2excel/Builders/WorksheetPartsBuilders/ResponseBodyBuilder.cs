﻿using ClosedXML.Excel;
using Microsoft.OpenApi.Models;

namespace OpenApi2Excel.Builders.WorksheetPartsBuilders;

internal class ResponseBodyBuilder(
    RowPointer actualRow,
    int attributesColumnIndex,
    IXLWorksheet worksheet,
    OpenApiDocumentationOptions options) : WorksheetPartBuilder(actualRow, worksheet, options)
{
    public void AddResponseBodyPart(OpenApiOperation operation)
    {
        if (!operation.Responses.Any())
            return;

        Fill(1).WithText("RESPONSE").WithBoldStyle();
        AddEmptyRow();
        foreach (var response in operation.Responses)
        {
            AddResponseHttpCode(response.Key, response.Value.Description);
            AddReponseHeaders(response.Value.Headers);
            AddPropertiesTreeForMediaTypes(response.Value.Content, attributesColumnIndex);
        }
        AddEmptyRow();
    }

    private void AddReponseHeaders(IDictionary<string, OpenApiHeader> valueHeaders)
    {
        if (!valueHeaders.Any())
            return;

        AddEmptyRow();

        Fill(1).WithText("Response headers").WithBoldStyle();
        ActualRow.MoveNext();
        foreach (var openApiHeader in valueHeaders)
        {
            var nextCellNumber = Fill(1).WithText(openApiHeader.Key)
                .Next(attributesColumnIndex - 1).WithText(Options.Language.Get(openApiHeader.Value.Deprecated))
                .Next().WithText(Options.Language.Get(openApiHeader.Value.Required))
                .Next().WithText(openApiHeader.Value.Description)
                .Next().GetCellNumber();
            FillSchemaDescriptionCells(openApiHeader.Value.Schema, nextCellNumber);
            ActualRow.MoveNext();
        }
        ActualRow.MoveNext();
    }

    private void AddResponseHttpCode(string httpCode, string? description)
    {
        if (string.IsNullOrEmpty(description))
        {
            Fill(1).WithText($"Response HttpCode: {httpCode}").WithBoldStyle();
        }
        else
        {
            Fill(1).WithText($"Response HttpCode: {httpCode}: {description}").WithBoldStyle();
        }
        ActualRow.MoveNext();
    }
}