module.exports = function(grunt) {
  require('load-grunt-tasks')(grunt);
  var path = require('path');

  // Add in time grunt
  require('time-grunt')(grunt);

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    dest: grunt.option('target') || 'dist',
    basePath: 'App_Plugins/<%= pkg.name %>',

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
      options: {
        spawn: false,
        atBegin: true
      },

      less: {
        files: ['app/styles/**/*.less'],
        tasks: ['less:dist']
      },

      js: {
        files: ['app/scripts/**/*.js'],
        tasks: ['concat:dist']
      },

      html: {
        files: ['app/views/**/*.html'],
        tasks: ['copy:views']
      },

      trees: {
        files: ['app/tree/**/*'],
        tasks: ['copy:trees']
      },

      dll: {
        files: ['src/PhoenixConverters/bin/Debug/*.dll'],
        tasks: ['copy:dll']
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
        src: ['src/PhoenixConverters/PhoenixConverters.csproj'],
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

  grunt.registerTask('default', 'Concat files, build Less & copy config & views', function(){
    validateTarget();
    grunt.task.run(['concat', 'less', 'msbuild', 'copy:dll', 'copy:config', 'copy:views', 'copy:trees']);
  });

  grunt.registerTask('nuget', 'Clean, rebuild, copy files for nuget & create it', function(){
    validateTarget();
    grunt.task.run(['clean', 'default', 'copy:nuget', 'template:nuspec', 'mkdir:pkg', 'nugetpack']);
  });

  grunt.registerTask('package', 'Clean, rebuild, copy files for umbraco package & create it', function(){
    validateTarget();
    grunt.task.run(['clean', 'default', 'copy:umbraco', 'mkdir:pkg', 'umbracoPackage']);
  });

  
  //Validation for --target option
  function validateTarget() {
    var destTarget = grunt.config.get('dest');
    
    grunt.log.oklns('Target (dest) is: ' + destTarget);

    if(destTarget != 'dist') {
      if(destTarget === true) {
        grunt.fail.warn('You need to specify a folder for target: grunt --target=c:/mysite/');
      }

      if(!grunt.file.isDir(destTarget)){
        grunt.fail.warn('The target passed in is not a folder path.');
      }
    }
  }

};

