﻿    /// <reference path="C:\Users\Aleksandar\Desktop\SAP\SAPProject\SAP\Scripts/angular.js" />
var app = angular.module('SapApp', ['ngAnimate'], function ($locationProvider) {
    $locationProvider.html5Mode({ enabled: true, requireBase: false});
});

app.controller('FormController', function ($scope, $http, $location) {
    $scope.activeTab = 1;
    $scope.isAdding = false;
    $scope.hasValues = false;
    $scope.activePanel = -1;
    $scope.newQuestion = {};
    $scope.form = {};
    $scope.form.questions = [];
    $scope.survey = {};
    $scope.errorMessage = null;

    $scope.addQuestion = function () {
        $scope.isAdding = !$scope.isAdding;
        setEmptyObject();
    };

    $scope.loadSurvey = function () {
        var id = $location.path().split('/')[3];
        if (id != undefined) {
            $http.get("/api/Surveys/" + id)
            .then(function (response) {
                if (response.status == 400) {
                    $scope.errorMessage = data;
                } else {
                    $scope.survey.survey = response.data;
                    console.log(response.data);
                    $scope.form.questions = $scope.survey.survey.Questions;
                    $scope.form.title = $scope.survey.survey.Name;
                }
            }, function (response, data) {
                $scope.errorMessage = data;
            });
        }
    };
    $scope.deleteQuestion = function (id) {
        var del = confirm("Are you sure you want to delete the question?");
        console.log(id);
        if (del) {
            if (id != undefined) {
                $http.delete("/api/Questions/" + $scope.survey.survey.Survey_type + "/" + id)
                    .then(function (response) {
                        if (response.status == 400) {
                            $scope.errorMessage = data;
                        } else {
                            $scope.loadSurvey();
                        }
                    }, function (response) {
                        $scope.errorMessage = response;
                    });
            }
        }
    }

    $scope.confirmQuestion = function () {
        console.log($scope.newQuestion);
        if ($scope.newQuestion.type != 'text' && $scope.newQuestion.type != 'textarea' && $scope.newQuestion.type != '') {
            if ($scope.newQuestion.values == null || $scope.newQuestion.values.length == 0){
                alert("No value options added!");
            }
            else {
                $scope.isAdding = false;
                $http.post("/api/Questions/" + $scope.survey.survey.Survey_type, JSON.stringify($scope.newQuestion))
                .then(function (response) {
                    if (response.status == 400) {
                        $scope.errorMessage = data;
                    } else {
                        $scope.loadSurvey();
                    }
                }, function (response) {
                    $scope.errorMessage = response;
                });
                setEmptyObject();
            }
        } else {
            $scope.isAdding = false;
            //console.log("HERE");
            $http.post("/api/Questions/" + $scope.survey.survey.Survey_type, JSON.stringify($scope.newQuestion))
            .then(function (response) {
                if (response.status == 400) {
                    $scope.errorMessage = data;
                } else {
                    $scope.loadSurvey();
                }
            }, function (response) {
                $scope.errorMessage = response;
            });
            setEmptyObject();
        }
        
    };

    $scope.addValue = function (value) {
        console.log(value);
        
        if ($scope.newQuestion.values == null) {
            $scope.newQuestion.values = [];
            $scope.newQuestion.values.push(value);
            $scope.value = "";
        } else {
            $scope.newQuestion.values.push(value);
            $scope.value = "";
        }
        $scope.value = "";
        console.log($scope.newQuestion.values);
        
    };

    $scope.removeValue = function (index) {
        if (index !== undefined) {
            $scope.newQuestion.values.splice(index, 1);
        }
    };

    $scope.cancelQuestion = function () {
        var answer = confirm("Are you sure you want to cancel?");
        if (answer) {
            $scope.isAdding = false;
            setEmptyObject();
        }
    };

    $scope.editQuestion = function (id) {
        //console.log(id);
        $scope.isAdding = !$scope.isAdding;
        if (id != undefined) {
            //console.log($scope.form.questions);
            $http.get("/api/Questions/" + $scope.survey.survey.Survey_type + "/" + id)
                .then(function (response) {
                    if (response.status == 400) {
                        $scope.errorMessage = response;
                    } else {
                        console.log(response.data);
                        $scope.newQuestion = response.data;
                        //setEmptyObject();
                    }
                }, function (response) {
                    $scope.errorMessage = response;
                });
            //$scope.newQuestion = $scope.form.questions.find()
        }
    }
    $scope.typeChange = function () {
        if ($scope.newQuestion.type == "text" || $scope.newQuestion.type == "textarea" || $scope.newQuestion.type == null) {
            $scope.newQuestion.values = [];
            $scope.hasValues = false;
        }
        else {
            $scope.hasValues = true;
        }
    };

    $scope.showPanel = function (panel) {
        if (panel == $scope.activePanel) {
            $scope.activePanel = -1;
        } else {
            $scope.activePanel = panel;
        }
    };

    $scope.showValuesForm = function () {
        return false;
    }

    $scope.isShowing = function (panel) {
        return $scope.activePanel === panel;
    };

    $scope.setTab = function (tab) {
        $scope.activeTab = tab;
    };

    $scope.isSet = function (tab) {
        return $scope.activeTab === tab;
    };

    function setEmptyObject() {
        $scope.newQuestion = {
            title: "",
            type: "",
            required: false,
            description: "",
            values: [],
            survey_id: $scope.survey.survey.Id
        };
    }
});

app.controller('FillOutController', function ($scope, $http, $location, $window) {
    $scope.form = {};
    $scope.answers = [];
    $scope.checkboxes = [];
    $scope.survey_id = 0;
    $scope.loadQuestions = function () {
        var id = $location.search().survey_id;
        console.log(id);
        if (id != undefined) {
            $http.get("/api/Surveys/1/" + id)
            .then(function (response) {
                if (response.status == 400) {
                    $scope.errorMessage = data;
                } else {
                    var data = response.data;
                    console.log(data);
                    $scope.survey_id = data.Id;
                    $scope.form.questions = data.Questions;
                }
            }, function (response, data) {
                $scope.errorMessage = data;
            });
        }
    };

    $scope.submit = function () {
        var answers = [];
        for (var i = 0; i < $scope.form.questions.length; i++) {
            if ($scope.form.questions[i].type == "checkbox") {
                for (var j = 0; j < $scope.form.questions[i].values.length; j++) {
                    var id = $scope.form.questions[i].values[j];
                    if ($scope.checkboxes[id] != undefined && $scope.checkboxes[id] == true) {
                        //console.log($scope.form.questions[i].values[j]);
                        answers.push({ QuestionId: $scope.form.questions[i].Id, Answer: $scope.form.questions[i].values[j] });
                    }
                }
            } else {
                answers.push({ QuestionId: $scope.form.questions[i].Id, Answer: $scope.form.questions[i].answer });
            }
            
        }
        var data = { survey_id: $scope.survey_id, answers: answers };
        $http.post("/Surveys/FillOut", JSON.stringify(data))
        .then(function () {
            $window.location.href = "/Account/Login/?fill=true";
        });
    }
});


app.controller('fController', function ($scope, $http, $location, $window) {
    $scope.form = {};
    $scope.answers = [];
    $scope.checkboxes = [];
    $scope.survey_id = 0;
    $scope.loadQuestions = function (id) {
        console.log(id);
        if (id != undefined) {
            $http.get("/api/Surveys/2/" + id)
            .then(function (response) {
                if (response.status == 400) {
                    $scope.errorMessage = data;
                } else {
                    var data = response.data;
                    console.log(data);
                    $scope.survey_id = data.Id;
                    $scope.form.questions = data.Questions;
                }
            }, function (response, data) {
                $scope.errorMessage = data;
            });
        }
    };

    $scope.submit = function () {
        var answers = [];
        for (var i = 0; i < $scope.form.questions.length; i++) {
            if ($scope.form.questions[i].type == "checkbox") {
                for (var j = 0; j < $scope.form.questions[i].values.length; j++) {
                    var id = $scope.form.questions[i].values[j];
                    if ($scope.checkboxes[id] != undefined && $scope.checkboxes[id] == true) {
                        //console.log($scope.form.questions[i].values[j]);
                        answers.push({ QuestionId: $scope.form.questions[i].Id, Answer: $scope.form.questions[i].values[j] });
                    }
                }
            } else {
                answers.push({ QuestionId: $scope.form.questions[i].Id, Answer: $scope.form.questions[i].answer });
            }

        }
        var data = { survey_id: $scope.survey_id, answers: answers };
        $http.post("/f/FillOut", JSON.stringify(data))
        .then(function () {
            $window.location.href = "/f/ThankYou";
        });
    }
});

app.controller("LinkController", function ($scope, $http, $location) {
    $scope.path = null;
    $scope.isShowingErr = false;
    $scope.isShowing = false;
    $scope.email = null;
    $scope.UID = null;
    $scope.getLink = function (id) {
        if (id != undefined) {
            if ($scope.UID == null) {
                $http.get("/api/surveys/link/" + id)
                .then(function (response) {
                    $scope.path = $location.protocol() + "://" + location.host + "/f/s/" + response.data.UID;
                    $scope.UID = response.data.UID;
                    $scope.surveyName = response.data.Survey.Name;
                });
            }

        } else {
            
        }
    }
    $scope.send = function () {
        if (!validateEmail($scope.email) && $scope.UID == null) {
            $scope.isShowingErr = true;
            $scope.error = "Please input a valid email address & generate a link";
            return;
        } 
        if ($scope.UID == null) {
            $scope.isShowingErr = true;
            $scope.error = "Please generate a link";
            return;
        }
        if (!validateEmail($scope.email)) {
            $scope.isShowingErr = true;
            $scope.error = "Please input a valid email address";
            return;
        }
        var data = { UID: $scope.UID, Email: $scope.email, SurveyId: $scope.SurveyId, URL : $scope.path };
        $scope.isShowingErr = false;
        $http.post("/api/surveys/link", JSON.stringify(data))
        .then(function () {
            $scope.alert = "Email to " + $scope.email + " sent successfully";
            $scope.email = null;
            $scope.isShowing = true;
        });
        
    }

    $scope.setSurveyId = function (id) {
        if (id != undefined) {
            $scope.SurveyId = id;
        }
    }

    $scope.closeAlert = function () {
        $scope.isShowing = !$scope.isShowing;
    }

    function validateEmail(email) {
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }
});

app.directive('selectOnClick', function () {
    // Linker function
    return function (scope, element, attrs) {
        element.bind('click', function () {
            this.select();
        });
    };
});

