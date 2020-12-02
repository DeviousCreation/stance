const 
    path = require('path'),
    webpack = require('webpack'),
    MiniCssExtractPlugin = require('mini-css-extract-plugin'),
    CopyWebpackPlugin = require('copy-webpack-plugin'),
    {CleanWebpackPlugin} = require('clean-webpack-plugin'),
    CompressionPlugin = require('compression-webpack-plugin');

module.exports = {
    entry: {
        theme: ['./Resources/Styles/theme.scss'],
        'vendors-styles': './Resources/Styles/vendors.scss',
        app: ['./Resources/Scripts/app.ts', './Resources/Styles/app.scss'],
        'users-list': './Resources/Scripts/pages/users-list/users-list.ts',
        'user-view': './Resources/Scripts/pages/user-view/user-view.ts',
        'role-create': './Resources/Scripts/pages/role-create/role-create.ts',
        'role-edit': './Resources/Scripts/pages/role-edit/role-edit.ts',
        'role-listing': './Resources/Scripts/pages/role-list/role-list.ts',
        'role-view': './Resources/Scripts/pages/role-view/role-view.ts',
        'profile-device': './Resources/Scripts/pages/profile-device/profile-device.ts',
        'profile-app': './Resources/Scripts/pages/profile-app/profile-app.ts',
        'validate-device-mfa': './Resources/Scripts/pages/validate-device-mfa/validate-device-mfa.ts',
        'validate-email-mfa': './Resources/Scripts/pages/validate-email-mfa/validate-email-mfa.ts',    
        'notification-list': './Resources/Scripts/pages/notification-list/notification-list.ts',    
        'system-alert-list': './Resources/Scripts/pages/system-alert-list/system-alert-list.ts',    
        'tenant-list': './Resources/Scripts/pages/tenant-list/tenant-list.ts',    
        'tenant-create': './Resources/Scripts/pages/tenant-create/tenant-create.ts',    
        'tenant-view': './Resources/Scripts/pages/tenant-view/tenant-view.ts',    
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot')
    },
    resolve: {
        extensions: ['.ts', '.tsx', '.js']
    },
    module: {
        rules: [
            { 
                test: /dataTables\.net.*/, 
                use: 'imports-loader?define=>false,$=jquery'
            },
            {
                test: /\.tsx?$/,
                use: 'ts-loader'
            },
            {
                test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
                 use: 'file-loader'                
            },
            {
                test: /theme\.s[ac]ss$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',                    
                    'sass-loader'
                ]
            }, 
            {
                test: /\.s[ac]ss$/i,
                exclude: /theme\.s[ac]ss$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader', 
                    'resolve-url-loader',             
                    'sass-loader'
                ]
            },            
            {
                test: /\.(png|svg|jpg|gif)$/,
                use: [
                    {
                        loader: 'url-loader',
                        options: {
                            limit: 1
                        }
                    }
                ]
            }
        ]
    },
    optimization: {
         splitChunks: {
            cacheGroups: {
             vendors: {
                 test: /[\\/]node_modules[\\/]/,
                 name: 'vendors',
                 chunks: 'all'
               },
             commons: {
                 name: 'commons',
                 test: /[\\/]Scripts[\\/](providers)[\\/]/,
                 chunks: 'all'
               },
              
         }
         }
       },
    plugins: [
        new CleanWebpackPlugin(),
        new MiniCssExtractPlugin({
            filename: '[name].css',
            chunkFilename: '[id].css',
            ignoreOrder: false
        }),
        new CopyWebpackPlugin({
            patterns: [
            './Resources/Assets/logo.png',
            './Resources/Assets/logo-text.png',
            './Resources/Assets/logo-icon.png',
            './Resources/Assets/android-chrome-192x192.png',
            './Resources/Assets/android-chrome-512x512.png',
            './Resources/Assets/apple-touch-icon.png',
            './Resources/Assets/favicon.ico',
            './Resources/Assets/favicon-16x16.png',
            './Resources/Assets/favicon-32x32.png',
            './Resources/Assets/site.webmanifest',
        ]}),
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            'window.jQuery': 'jquery',
            'window.$': 'jquery',
        }),
        new CompressionPlugin()
    ]
};