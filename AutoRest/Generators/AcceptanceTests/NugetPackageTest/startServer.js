﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

var util = require('util');
var fs = require('fs');
var child_process = require('child_process');
require('shelljs/global');

var child;

before(function (done) {
  var started = false;
  var out = fs.openSync('./server.log', 'w');
  fs.writeSync(out, 'Test run started at ' + new Date().toISOString() + '\n');

  var nodePath = which('node') || which('node.exe');

  child = child_process.spawn(nodePath, [__dirname + '/../server/startup/www']);

  child.stdout.on('data', function (data) {
    fs.writeSync(out, data.toString('UTF-8'));
    if (data.toString().indexOf('started') > 0) {
      started = true;
      done();
    }
  });

  child.on('close', function (code) {
    if (!started) {
      done();
    }
  });
});

after(function (done) {
  child.kill();
  done();
});
