const path = require('path');

module.exports = {
    // multiple entry points - https://github.com/webpack/docs/wiki/multiple-entry-points
    entry: {
        main: ['babel-polyfill', path.resolve(__dirname, 'src/meter_dashboard/index.ts')],
        ping: ['babel-polyfill', path.resolve(__dirname, 'src/scada_ping_status/index.ts')],
        scada: ['babel-polyfill', path.resolve(__dirname, 'src/scada_dashboard/scada.ts')],
        wbes_archive: ['babel-polyfill', path.resolve(__dirname, 'src/wbes_arch_dashboard/wbes_arch.ts')],
        state_demands: ['babel-polyfill', path.resolve(__dirname, 'src/state_demands_dashboard/state_demands.ts')],
        dem_freq: ['babel-polyfill', path.resolve(__dirname, 'src/dem_freq_dashboard/dem_freq.ts')],
        isgs_stats: ['babel-polyfill', path.resolve(__dirname, 'src/isgs_stats/index.ts')],
        pmu_freq: ['babel-polyfill', path.resolve(__dirname, 'src/pmu_freq_dashboard/index.ts')]
    },

    output: {
        filename: "[name].js"
    },

    // https://webpack.js.org/configuration/externals/
    externals: {
        'plotly.js-dist': 'Plotly',
        jquery: 'jQuery'
    },

    // Enable sourcemaps for debugging webpack's output.
    devtool: "source-map",

    module: {
        rules: [
            {
                test: /\.ts$/,
                exclude: /node_modules/,
                use: ["babel-loader", "ts-loader"]
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: ["babel-loader"]
            },
            // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
            {
                enforce: "pre",
                test: /\.js$/,
                loader: "source-map-loader"
            }
        ]
    },

    plugins: [],

    resolve: {
        extensions: ['.js', '.ts'],
    }
};