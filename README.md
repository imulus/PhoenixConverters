PhoenixConverters
=================

Very much a WIP.

This repo is for the concept of DataTypeConverters in Umbraco v7.

http://bit.ly/1gxR49z

## Developing ##

### Install Dependencies ###

    npm install
    npm install -g grunt && npm install -g grunt-cli

### Build ###

    grunt
Builds the project to `/dist/`.  These files can be dropped into an Umbraco 7 site, or you can build directly to a site using:

    grunt --target="D:\inetpub\mysite"

You can also watch for changes using:

    grunt watch
    grunt watch --target="D:\inetpub\mysite"
