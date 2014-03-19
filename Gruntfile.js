module.exports = function(grunt) {
  require('load-grunt-tasks')(grunt);
  var path = require('path');

  // Add in time grunt
  require('time-grunt')(grunt);

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    dest: grunt.option('target') || 'dist',
    basePath: 'App_Plugins/<%= pkg.name %>',
    csProj: 'src/PhoenixConverters/PhoenixConverters.csproj',

    concat: {
      dist: {
        src: [
          'app/scripts/controllers/edit.controller.js',
          'app/scripts/services/converter.service.js'
        ],
        dest: '<%= dest %>/<%= basePath %>/js/phoenix.js',
        nonull: true
      }
    },

    less: {
      dist: {
        options: {
          paths: ["app/styles"],
        },
        files: {
          '<%= dest %>/<%= basePath %>/css/phoenix.css': 'app/styles/phoenix.less',
        }
      }
    },

    watch: {
      less: {
        files: ['app/styles/**/*.less'],
        tasks: ['less:dist'],
        options: {
          spawn: false
        }
      },

      js: {
        files: ['app/scripts/**/*.js'],
        tasks: ['concat:dist'],
        options: {
          spawn: false
        }
      },

      html: {
        files: ['app/views/**/*.html'],
        tasks: ['copy:views'],
        options: {
          spawn: false
        }
      },

      trees: {
        files: ['app/tree/**/*'],
        tasks: ['copy:trees']
      },
      dll: {
        files: ['src/PhoenixConverters/bin/Debug/*.dll'],
        tasks: ['copy:dll'],
        options: {
          spawn: false
        }
      }
    },

    copy: {
      config: {
        src: 'config/package.manifest',
        dest: '<%= dest %>/<%= basePath %>/package.manifest',
      },      

      views: {
        expand: true,
        cwd: 'app/views/',
        src: '**',
        dest: '<%= dest %>/<%= basePath %>/views/'
      },

      trees: {
        expand: true,
        cwd: 'app/tree/',
        src: '**',
        dest: '<%= dest %>/<%= basePath %>/backoffice/phoenixTree/'

      },

      dll: {
        expand: true,
        flatten: true,
        src: 'src/PhoenixConverters/bin/Debug/*.dll',
        dest: '<%= dest %>/bin/'
      },

      nuget: {
        expand: true,
        cwd: '<%= dest %>',
        src: '**',
        dest: 'tmp/nuget/content/'
      },

      umbraco: {
        expand: true,
        cwd: '<%= dest %>/',
        src: '**',
        dest: 'tmp/umbraco/'
      },

    },

    msbuild: {
      options: {
        stdout: true,
        verbosity: 'quiet',
      },
      dist: {
        src: ["<%= csProj %>"],
        options: {
          projectConfiguration: 'Debug',
          targets: ['Clean', 'Rebuild']
        }
      }
    },

    template: {
      nuspec: {
        options: {
          data: {
            name:        '<%= pkg.name %>',
            version:     '<%= pkg.version %>',
            author:      '<%= pkg.author.name %>',
            description: '<%= pkg.description %>'
          }
        },
        files: {
          'tmp/nuget/<%= pkg.name %>.nuspec': 'config/package.nuspec'
        }
      }
    },

    mkdir: {
      pkg: {
        options: {
          create: ['pkg/nuget', 'pkg/umbraco']
        },
      },
    },

    nugetpack: {
      dist: {
        src: 'tmp/nuget/<%= pkg.name %>.nuspec',
        dest: 'pkg/nuget/'
      }
    },

    umbracoPackage: {
      options: {
        name:        '<%= pkg.name %>',
        version:     '<%= pkg.version %>',
        url:         '<%= pkg.url %>',
        license:     '<%= pkg.license %>',
        licenseUrl:  '<%= pkg.licenseUrl %>',
        author:      '<%= pkg.author %>',
        authorUrl:   '<%= pkg.authorUrl %>',
        manifest:    'config/package.xml',
        readme:      'config/readme.txt',
        sourceDir:   'tmp/umbraco',
        outputDir:   'pkg/umbraco',
      }
    },

    clean: {
      dist: '<%= dest %>'
    }
  });

  //Legacy - now below so we can run our validation check first...
  //grunt.registerTask('default', ['concat', 'less', 'copy:config', 'copy:views']);
  //grunt.registerTask('nuget', ['clean', 'default', 'copy:nuget', 'template:nuspec', 'mkdir:pkg', 'nugetpack']);
  //grunt.registerTask('package', ['clean', 'default', 'copy:umbraco', 'mkdir:pkg', 'umbracoPackage']);

  //TASK: default
  grunt.registerTask('default', 'Concat files, build Less & copy config & views', function(){
    validateTarget();
    grunt.task.run(['concat', 'less', 'msbuild', 'copy:dll', 'copy:config', 'copy:views', 'copy:trees']);
  });

  //TASK nuget
  grunt.registerTask('nuget', 'Clean, rebuild, copy files for nuget & create it', function(){
    validateTarget();
    grunt.task.run(['clean', 'default', 'copy:nuget', 'template:nuspec', 'mkdir:pkg', 'nugetpack']);
  });


  //TASK umbraco
  grunt.registerTask('package', 'Clean, rebuild, copy files for umbraco package & create it', function(){
    validateTarget();
    grunt.task.run(['clean', 'default', 'copy:umbraco', 'mkdir:pkg', 'umbracoPackage']);
  });


  
  //Validation for --target option
  function validateTarget() {

    var destTarget = grunt.config.get('dest');

    //Debug
    grunt.log.oklns('Target (dest) is: ' + destTarget);

    //If dest is not set to dist then a target option was set
    if(destTarget != 'dist') {

      //Happens when grunt --target is called
      if(destTarget === true) {
        //Error message & stop processing task
        grunt.fail.warn('You need to specify a folder for target: grunt --target=c:/mysite/');
      }

      //Ensure that the dest value the --target is actually a directory that exists
      if(!grunt.file.isDir(destTarget)){

        //Error message & stop processing task
        grunt.fail.warn('The target passed in is not a folder path.');
      }
    }
  }

};

