angular.module('umbraco.services').factory('phoenixConverterService', function ($http, umbRequestHelper) {
    var service = {
        getConverterByAlias: function (alias) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/PhoenixApi/GetConverterByAlias/" + alias), 'Failed to retrieve converter'
            );
        },

        getSourceDataTypes: function() {
            // Hack - grab DataTypes from Tree API, as `dataTypeService.getAll()` isn't implemented yet
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/UmbracoTrees/DataTypeTree/GetNodes?id=-1&application=developer&tree=&isDialog=false"), 'Failed to retrieve source datatypes from tree service'
            ).then(function (data) {
                console.log(data);
                return data.map(function(d) {
                    return { "id": d.id, "name": d.name }
                }); 
            });
        },

        getTargetDataTypes: function (dataTypeAlias) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/PhoenixApi/DataType/GetDataTypesByAlias/" + dataTypeAlias), 'Failed to retrieve target datatypes from API'
            ).then(function (data) {
                console.log(data);
                return data.map(function (d) {
                    return { "id": d.id, "name": d.name }
                });
            });
        },

        test: function (alias, sourceDataTypeId) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/PhoenixApi/Perform/Test/" + alias + "/" + sourceDataTypeId), 'Failed to retrieve converter'
            );
        }
    }

    return service;
});