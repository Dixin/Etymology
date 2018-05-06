const webpack = require("webpack");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");

const merge = require("webpack-merge");
const common = require("./webpack.config.js");

const Paths = common.Paths;
delete common.Paths;

module.exports = merge(common, {
    mode: "production",
    plugins: [
        new HtmlWebpackPlugin({
            template: Paths.indexTemplate,
            filename: Paths.indexHTML,
            inject: "body",
            showErrors: true,
            minify: {
                removeComments: true,
                collapseWhitespace: true
            }
        }),
        new webpack.ProvidePlugin({
            jQuery: "jquery"
        })
    ],
    optimization: {
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /[\\/]node_modules[\\/]/,
                    name: 'vendors',
                    chunks: 'all',
                },
            },
        },
        minimizer: [
            new UglifyJsPlugin({
                uglifyOptions: {
                    compress: true,
                    output: {
                        comments: false
                    }
                }
            })
        ]
    }
});
