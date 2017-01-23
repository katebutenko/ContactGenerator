/// <binding BeforeBuild='clean' Clean='clean' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

"use strict";

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  uglify = require("gulp-uglify");

var paths = {
  webroot: "./"
};

paths.importantJs = paths.webroot + "lib/*.js";
paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "scripts/site.min.js";
paths.concatCssDest = paths.webroot + "scripts/site.min.css";


var itemsToCopy = {
  './node_modules/ammap3/ammap/ammap.js': paths.webroot + 'lib',
  './node_modules/ammap3/ammap/maps/js/worldLow.js': paths.webroot + 'lib',
  './node_modules/ammap3/ammap/themes/chalk.js': paths.webroot + 'lib',  
}

var jQueryPath = './node_modules/jquery/dist/jquery.min.js' ;

gulp.task("copyjQuery", function () {
  
    return gulp.src(jQueryPath)
    .pipe(gulp.dest("./scripts"));
  
});

gulp.task("copy", function () {
  for (var src in itemsToCopy) {
    if (!itemsToCopy.hasOwnProperty(src)) continue;
    gulp.src(src)
    .pipe(gulp.dest(itemsToCopy[src]));
  }
});

gulp.task("clean:js", function (cb) {
  rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
  rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
  return gulp.src([paths.importantJs, paths.js, "!" + paths.minJs], { base: "." })
    .pipe(concat(paths.concatJsDest))
    .pipe(uglify())
    .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
  return gulp.src([paths.css, "!" + paths.minCss])
    .pipe(concat(paths.concatCssDest))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);

gulp.task('build', ['copy', 'min']);

gulp.task('default', ["build"]);