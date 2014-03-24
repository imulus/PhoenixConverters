angular.module('umbraco.services').factory('phoenixConverterService', function ($http, umbRequestHelper) {
    var service = {
        getConverterByAlias: function (alias) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/PhoenixApi/GetConverterByAlias/" + alias), 'Failed to retrieve converter'
            );
        },

        getSourceDataTypes: function() {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/PhoenixApi/DataType/GetSourceDataTypes"), 'Failed to retrieve source datatypes from API'
            ).then(function (data) {
                return data.map(function(d) {
                    return { "id": d.id, "name": d.name }
                }); 
            });
        },

        getTargetDataTypes: function (dataTypeAlias) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/PhoenixApi/DataType/GetDataTypesByAlias/" + dataTypeAlias), 'Failed to retrieve target datatypes from API'
            ).then(function (data) {
                return data.map(function (d) {
                    return { "id": d.id, "name": d.name }
                });
            });
        },

        test: function (alias, sourceDataTypeId) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/PhoenixApi/Perform/Test/" + alias + "/" + sourceDataTypeId), 'Failed to test'
            );
        },

        convert: function (alias, sourceDataTypeId) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/PhoenixApi/Perform/Conversion/" + alias + "/" + sourceDataTypeId), 'Failed to convert'
            );
        }
    }

    return service;
});