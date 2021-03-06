'use strict';

var util = require('util');

var models = require('./index');

/**
 * @class
 * Initializes a new instance of the ClassOptionalWrapper class.
 * @constructor
 */
function ClassOptionalWrapper() { }

/**
 * Validate the payload against the ClassOptionalWrapper schema
 *
 * @param {JSON} payload
 *
 */
ClassOptionalWrapper.prototype.validate = function (payload) {
  if (!payload) {
    throw new Error('ClassOptionalWrapper cannot be null.');
  }
  if (payload['value']) {
    models['Product'].validate(payload['value']);
  }
};

/**
 * Deserialize the instance to ClassOptionalWrapper schema
 *
 * @param {JSON} instance
 *
 */
ClassOptionalWrapper.prototype.deserialize = function (instance) {
  if (instance) {
    if (instance.value !== null && instance.value !== undefined) {
      instance.value = models['Product'].deserialize(instance.value);
    }
  }
  return instance;
};

module.exports = new ClassOptionalWrapper();
