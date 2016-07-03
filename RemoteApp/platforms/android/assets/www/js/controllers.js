angular.module('remote.controllers', [])

.controller('login', ['$scope', '$rootScope','$timeout',
    function ($scope, $rootScope, $timeout) {

        function SHA1(msg) {
            /**
            *  Secure Hash Algorithm (SHA1)
            *  http://www.webtoolkit.info/
            **/
            function rotate_left(n, s) {
                var t4 = (n << s) | (n >>> (32 - s));
                return t4;
            };
            function lsb_hex(val) {
                var str = "";
                var i;
                var vh;
                var vl;
                for (i = 0; i <= 6; i += 2) {
                    vh = (val >>> (i * 4 + 4)) & 0x0f;
                    vl = (val >>> (i * 4)) & 0x0f;
                    str += vh.toString(16) + vl.toString(16);
                }
                return str;
            };
            function cvt_hex(val) {
                var str = "";
                var i;
                var v;
                for (i = 7; i >= 0; i--) {
                    v = (val >>> (i * 4)) & 0x0f;
                    str += v.toString(16);
                }
                return str;
            };
            function Utf8Encode(string) {
                string = string.replace(/\r\n/g, "\n");
                var utftext = "";
                for (var n = 0; n < string.length; n++) {
                    var c = string.charCodeAt(n);
                    if (c < 128) {
                        utftext += String.fromCharCode(c);
                    }
                    else if ((c > 127) && (c < 2048)) {
                        utftext += String.fromCharCode((c >> 6) | 192);
                        utftext += String.fromCharCode((c & 63) | 128);
                    }
                    else {
                        utftext += String.fromCharCode((c >> 12) | 224);
                        utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                        utftext += String.fromCharCode((c & 63) | 128);
                    }
                }
                return utftext;
            };
            var blockstart;
            var i, j;
            var W = new Array(80);
            var H0 = 0x67452301;
            var H1 = 0xEFCDAB89;
            var H2 = 0x98BADCFE;
            var H3 = 0x10325476;
            var H4 = 0xC3D2E1F0;
            var A, B, C, D, E;
            var temp;
            msg = Utf8Encode(msg);
            var msg_len = msg.length;
            var word_array = new Array();
            for (i = 0; i < msg_len - 3; i += 4) {
                j = msg.charCodeAt(i) << 24 | msg.charCodeAt(i + 1) << 16 |
                msg.charCodeAt(i + 2) << 8 | msg.charCodeAt(i + 3);
                word_array.push(j);
            }
            switch (msg_len % 4) {
                case 0:
                    i = 0x080000000;
                    break;
                case 1:
                    i = msg.charCodeAt(msg_len - 1) << 24 | 0x0800000;
                    break;
                case 2:
                    i = msg.charCodeAt(msg_len - 2) << 24 | msg.charCodeAt(msg_len - 1) << 16 | 0x08000;
                    break;
                case 3:
                    i = msg.charCodeAt(msg_len - 3) << 24 | msg.charCodeAt(msg_len - 2) << 16 | msg.charCodeAt(msg_len - 1) << 8 | 0x80;
                    break;
            }
            word_array.push(i);
            while ((word_array.length % 16) != 14) word_array.push(0);
            word_array.push(msg_len >>> 29);
            word_array.push((msg_len << 3) & 0x0ffffffff);
            for (blockstart = 0; blockstart < word_array.length; blockstart += 16) {
                for (i = 0; i < 16; i++) W[i] = word_array[blockstart + i];
                for (i = 16; i <= 79; i++) W[i] = rotate_left(W[i - 3] ^ W[i - 8] ^ W[i - 14] ^ W[i - 16], 1);
                A = H0;
                B = H1;
                C = H2;
                D = H3;
                E = H4;
                for (i = 0; i <= 19; i++) {
                    temp = (rotate_left(A, 5) + ((B & C) | (~B & D)) + E + W[i] + 0x5A827999) & 0x0ffffffff;
                    E = D;
                    D = C;
                    C = rotate_left(B, 30);
                    B = A;
                    A = temp;
                }
                for (i = 20; i <= 39; i++) {
                    temp = (rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 0x6ED9EBA1) & 0x0ffffffff;
                    E = D;
                    D = C;
                    C = rotate_left(B, 30);
                    B = A;
                    A = temp;
                }
                for (i = 40; i <= 59; i++) {
                    temp = (rotate_left(A, 5) + ((B & C) | (B & D) | (C & D)) + E + W[i] + 0x8F1BBCDC) & 0x0ffffffff;
                    E = D;
                    D = C;
                    C = rotate_left(B, 30);
                    B = A;
                    A = temp;
                }
                for (i = 60; i <= 79; i++) {
                    temp = (rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 0xCA62C1D6) & 0x0ffffffff;
                    E = D;
                    D = C;
                    C = rotate_left(B, 30);
                    B = A;
                    A = temp;
                }
                H0 = (H0 + A) & 0x0ffffffff;
                H1 = (H1 + B) & 0x0ffffffff;
                H2 = (H2 + C) & 0x0ffffffff;
                H3 = (H3 + D) & 0x0ffffffff;
                H4 = (H4 + E) & 0x0ffffffff;
            }
            var temp = cvt_hex(H0) + cvt_hex(H1) + cvt_hex(H2) + cvt_hex(H3) + cvt_hex(H4);

            return temp.toLowerCase();
        }

        $scope.username = window.localStorage.getItem("username");
        $rootScope.address = window.localStorage.getItem("address");
        $rootScope.sentinel = window.localStorage.getItem("sentinel");
        $rootScope.port = window.localStorage.getItem("port");

        $scope.logIn = function (user, pass, ip, port, sentinel) {
            if (ip == undefined) {
                $rootScope.address = 'localhost';
            }
            else {
                $rootScope.address = ip;
                window.localStorage.setItem("address", ip);
            }
            if (port == undefined) {
                $rootScope.port = '8088';
            }
            else {
                $rootScope.port = port;
                window.localStorage.setItem("port", port);
            }

            $scope.username = user;
            window.localStorage.setItem("username",user);
            $scope.password = pass;

            $rootScope.sentinel = sentinel;
            window.localStorage.setItem("sentinel" ,sentinel);

            var key = user + pass;
            var hash = SHA1(key);
            $rootScope.accessKey = hash;
            $rootScope.loggedIn = true;
            $scope.afterLogin($timeout);

        };

        $scope.logOut = function () {
            $scope.loggedIn = false;

        };

        $scope.clear = function () {
            window.localStorage.removeItem("username");
            window.localStorage.removeItem("address");
            window.localStorage.removeItem("port");
            window.localStorage.removeItem("sentinel");
        };
    }])


.controller('AppCtrl', function ($scope, $ionicModal, $timeout) {

    // Form data for the login modal
    $scope.taskData = {};

    // Create the login modal that we will use later
    $ionicModal.fromTemplateUrl('templates/taskCreator.html', {
        scope: $scope
    }).then(function (modal) {
        $scope.modal = modal;
    });

    // Triggered in the login modal to close it
    $scope.closeTask = function () {
        $scope.modal.hide();
    };

    // Open the login modal
    $scope.taskCreator = function () {
        $scope.modal.show();
    };

    // Perform the login action when the user submits the login form
    $scope.doTask = function () {
        console.log('Doing task', $scope.taskData);


        // Simulate a login delay. Remove this and replace with your login
        // code if using a login system
        $timeout(function () {
            $scope.closeTask();
        }, 1000);
    };
})


.controller('MyController', ['$scope', '$ionicPopup',
        function ($scope, $ionicPopup) {

            if ($scope.loggedIn) {
                $scope.consumer.onUpdateStateObject(function (stateobject) {
                    if ($scope.remoteLoaded) {
                        $scope.volume.value = $scope.consumer.RemoteControl.VolumeLevel.Value.level;
                        $scope.brightness.value = $scope.consumer.RemoteControl.BrightnessLevel.Value;
                    };
                    if ($scope.mediaLoaded) {
                        if ($scope.position != undefined) {
                            $scope.position.value = $scope.consumer.MediaPlayer.TimeData.Value.currentPosition;
                        }
                    };
                });

                $scope.showConfirm = function () {
                    var confirmPopup = $ionicPopup.confirm({
                        title: 'Confirm',
                        template: 'Are you sure ?'
                    });

                    confirmPopup.then(function (res) {
                        if (res) {
                            $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "AnswerQuestion", "oui");
                        } else {
                            $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "AnswerQuestion", "non");
                        }
                    });
                };

                $scope.createTask = function (titre, description, jour, mois, annee, heure, minute) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "TaskCreator", [titre, description, jour, mois, annee, heure, minute]);
                };


                $scope.mute = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "SetVolume", "mute");
                };


                $scope.monitoroff = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "MonitorOff", "");
                };

                $scope.panicMode = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "PanicMode", "");
                };

                $scope.shutdown = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "Shutdown", "");
                    $scope.showConfirm();
                };

                $scope.sleep = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "Sleep", "");
                    $scope.showConfirm();
                };

                $scope.balance = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "SetPowerPlan", "balanced");
                };

                $scope.save = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "SetPowerPlan", "saver");
                };

                $scope.high = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "SetPowerPlan", "high");
                };

                $scope.reboot = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "Reboot", "");
                    $scope.showConfirm();
                };

                $scope.play = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "Play", "");
                };

                $scope.pause = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "Pause", "");
                };

                $scope.stop = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "Stop", "");
                };

                $scope.previous = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "Previous", "");
                };

                $scope.next = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "Next", "");
                };

                $scope.run = function () {
                    if ($scope.consumer.RemoteControl.MediaPlayerState.Value == false) {
                        $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "CloseMediaPlayer", "");
                    }
                    else {
                        $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "OpenMediaPlayer", "");
                    }
                };

                $scope.shuffle = function () {
                    $scope.consumer.sendMessageWithSaga({ Scope: "Package", Args: ["MediaPlayer"] }, "Shuffle", "set", function (result) {
                        $scope.shuffleState = (result.Data == true ? "off" : "on");
                    });
                };

                $scope.fullscreen = function () {
                    $scope.consumer.sendMessageWithSaga({ Scope: "Package", Args: ["MediaPlayer"] }, "FullScreen", "set", function (result) {
                        $scope.fullScreenState = (result.Data == true ? "off" : "on");
                    });
                };

                $scope.searchArtist = function (artist) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "LoadArtist", artist);
                };

                $scope.searchAlbum = function (album) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "LoadAlbum", album);
                };

                $scope.volume = {};

                $scope.SetVolume = function (rangeValue) {
                    console.log(rangeValue.value);
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "SetVolume", rangeValue.value);
                };

                $scope.brightness = {};
                $scope.SetBrightness = function (rangeValue) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "SetBrightness", rangeValue.value);
                };

                $scope.position = {};
                $scope.SetPosition = function (rangeValue) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "SetTime", rangeValue.value);
                };

                $scope.browse = function (URL) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "OpenBrowser", URL);
                };

                $scope.titleOnAlbumClick = function (song) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "LoadAlbum", song.Item2);
                    console.log(song.Item2);
                };

                $scope.titleOnTitleClick = function (song) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "LoadTitleFromPlaylist", song.Item3);
                };

                $scope.getVideos = function (search) {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["MediaPlayer"] }, "GetVideos", search);
                };

                $scope.openConstellation = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteControl"] }, "OpenBrowser", "https://developer.myconstellation.io/");
                };

                $scope.snapshot = function () {
                    $scope.consumer.sendMessage({ Scope: "Package", Args: ["RemoteWebcam"] }, "TakePicture", [$scope.manufacturer, $scope.model]);
                };

            }


        }]);
