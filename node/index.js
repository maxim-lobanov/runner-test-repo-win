var Spinner = require('cli-spinner').Spinner;

var spinner = new Spinner('processing.. %s');
spinner.setSpinnerString('|/-\\');
spinner.start();

setTimeout(() => {
    spinner.stop();
    console.log('\ndone')
}, 5000);