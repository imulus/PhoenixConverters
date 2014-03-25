angular.module("umbraco").controller('Phoenix.Tree.EditController', function ($scope, $routeParams, assetsService, phoenixConverterService) {

    $scope.isTesting = false;
    $scope.hasTested = false;

    //get the available source datatypes
    phoenixConverterService.getSourceDataTypes().then(function (data) {
        $scope.availableSourceDataTypes = data;
    });

    phoenixConverterService.getConverterByAlias($routeParams.id).then(function (data) {
        $scope.converter = {};
        $scope.converter.name = data.name; 
        $scope.converter.alias = data.alias;

        //get the available source datatypes
        phoenixConverterService.getTargetDataTypes(data.targetPropertyTypeAlias).then(function (data) {
            $scope.availableTargetDataTypes = data;
        });
    }); 

    $scope.test = function () {
        $scope.isTesting = true;

        phoenixConverterService.test($routeParams.id, $scope.sourceDataTypeId, $scope.targetDataTypeId).then(function (data) {
            console.log(data);
            $scope.testResults = data;
            $scope.isTesting = false;

            if ($scope.testResults.isCompatible) {
                $scope.hasTested = true;
            }
        });
    }

    $scope.convert = function() {
        $scope.isConverting = true;

        phoenixConverterService.convert($routeParams.id, $scope.sourceDataTypeId, $scope.targetDataTypeId).then(function (data) {
            $scope.testResults = data;
            $scope.isConverting = false;

            if ($scope.testResults.isCompatible) {
                $scope.hasConverted = true;
            }
        });
    }
  
    assetsService.loadCss("/App_Plugins/Phoenix/css/phoenix.css");
});
