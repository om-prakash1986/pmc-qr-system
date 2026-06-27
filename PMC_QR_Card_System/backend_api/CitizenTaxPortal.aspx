<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CitizenTaxPortal.aspx.cs" Inherits="CitizenTaxPortal" %>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Patna Nagar Nigam — Citizen Tax Portal</title>
    
    <!-- Premium Google Fonts -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous">
    <link href="https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700;800&display=swap" rel="stylesheet">
    
    <!-- Razorpay Checkout SDK -->
    <script src="https://checkout.razorpay.com/v1/checkout.js"></script>
    
    <style>
        :root {
            --primary: #0D3E73;
            --primary-light: #1e5fa6;
            --secondary: #E67E22;
            --secondary-light: #f39c12;
            --success: #2ECC71;
            --danger: #E74C3C;
            --bg-dark: #0a1128;
            --card-bg: rgba(255, 255, 255, 0.08);
            --card-border: rgba(255, 255, 255, 0.12);
            --text-light: #f8f9fa;
            --text-muted: #a0aec0;
            --glow-color: rgba(230, 126, 34, 0.4);
        }

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
            font-family: 'Outfit', sans-serif;
            -webkit-font-smoothing: antialiased;
        }

        body {
            background: radial-gradient(circle at 50% 10%, #152244 0%, #0a1128 70%);
            color: var(--text-light);
            min-height: 100vh;
            display: flex;
            flex-direction: column;
            align-items: center;
            overflow-x: hidden;
            padding: 20px;
        }

        /* ── CANVAS FOR CONFETTI ── */
        #confetti-canvas {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none;
            z-index: 9999;
        }

        .container {
            width: 100%;
            max-width: 520px;
            margin: 40px auto;
            position: relative;
            z-index: 10;
        }

        /* ── PORTAL HEADER ── */
        .portal-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .pmc-logo {
            width: 70px;
            height: 70px;
            border-radius: 50%;
            background: white;
            padding: 6px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
            margin-bottom: 12px;
            transition: transform 0.3s ease;
        }

        .pmc-logo:hover {
            transform: scale(1.1) rotate(5deg);
        }

        .portal-title {
            font-size: 24px;
            font-weight: 700;
            background: linear-gradient(135deg, #ffffff 40%, #ffc085 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            letter-spacing: 0.5px;
            text-transform: uppercase;
        }

        .portal-desc {
            font-size: 13px;
            color: var(--text-muted);
            margin-top: 6px;
            font-weight: 300;
        }

        /* ── GLASS CARD WRAPPER ── */
        .glass-card {
            background: var(--card-bg);
            border: 1px solid var(--card-border);
            border-radius: 20px;
            backdrop-filter: blur(20px);
            -webkit-backdrop-filter: blur(20px);
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
            padding: 30px 24px;
            width: 100%;
            transition: all 0.3s ease;
        }

        .hidden {
            display: none !important;
        }

        /* ── STEP CONTAINERS ── */
        .step-title {
            font-size: 18px;
            font-weight: 600;
            color: #fff;
            margin-bottom: 18px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .step-title svg {
            color: var(--secondary);
        }

        /* ── FORM ELEMENTS ── */
        .form-group {
            margin-bottom: 20px;
            position: relative;
        }

        .form-label {
            display: block;
            font-size: 12px;
            font-weight: 600;
            color: var(--text-muted);
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 8px;
        }

        .input-wrapper {
            position: relative;
            display: flex;
            align-items: center;
        }

        .input-wrapper svg {
            position: absolute;
            left: 14px;
            color: var(--text-muted);
            width: 18px;
            height: 18px;
        }

        .form-control {
            width: 100%;
            background: rgba(0, 0, 0, 0.25);
            border: 1px solid rgba(255, 255, 255, 0.15);
            border-radius: 10px;
            padding: 14px 14px 14px 44px;
            color: white;
            font-size: 15px;
            font-weight: 500;
            outline: none;
            transition: all 0.3s ease;
        }

        .form-control:focus {
            border-color: var(--secondary);
            box-shadow: 0 0 10px var(--glow-color);
            background: rgba(0, 0, 0, 0.35);
        }

        /* ── BUTTONS ── */
        .btn {
            width: 100%;
            background: linear-gradient(135deg, var(--secondary) 0%, #D35400 100%);
            border: none;
            border-radius: 10px;
            color: white;
            padding: 14px;
            font-size: 14px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 1px;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            box-shadow: 0 4px 15px rgba(230, 126, 34, 0.3);
            transition: all 0.3s ease;
        }

        .btn:hover:not(:disabled) {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(230, 126, 34, 0.5);
            background: linear-gradient(135deg, #f39c12 0%, #E67E22 100%);
        }

        .btn:active:not(:disabled) {
            transform: translateY(0);
        }

        .btn:disabled {
            opacity: 0.5;
            cursor: not-allowed;
            box-shadow: none;
        }

        .btn-outline {
            background: transparent;
            border: 1px solid rgba(255, 255, 255, 0.25);
            color: var(--text-light);
            box-shadow: none;
        }

        .btn-outline:hover:not(:disabled) {
            background: rgba(255, 255, 255, 0.05);
            border-color: rgba(255, 255, 255, 0.4);
            box-shadow: none;
        }

        /* ── ALERTS & TOASTS ── */
        .alert {
            display: flex;
            align-items: center;
            gap: 12px;
            border-radius: 10px;
            padding: 14px;
            font-size: 13px;
            margin-bottom: 20px;
            border: 1px solid transparent;
        }

        .alert-warning {
            background: rgba(243, 156, 18, 0.15);
            border-color: rgba(243, 156, 18, 0.25);
            color: #ffb84d;
        }

        .alert-error {
            background: rgba(231, 76, 60, 0.15);
            border-color: rgba(231, 76, 60, 0.25);
            color: #ff8080;
        }

        /* ── OTP BOXES ── */
        .otp-inputs {
            display: flex;
            justify-content: space-between;
            gap: 8px;
            margin-bottom: 15px;
        }

        .otp-box {
            width: 46px;
            height: 48px;
            background: rgba(0, 0, 0, 0.3);
            border: 1px solid rgba(255, 255, 255, 0.15);
            border-radius: 8px;
            color: white;
            font-size: 20px;
            font-weight: 700;
            text-align: center;
            outline: none;
            transition: all 0.3s ease;
        }

        .otp-box:focus {
            border-color: var(--secondary);
            box-shadow: 0 0 8px var(--glow-color);
            background: rgba(0, 0, 0, 0.4);
        }

        .sub-desc {
            font-size: 11.5px;
            color: var(--text-muted);
            text-align: center;
            margin-top: 10px;
            font-weight: 300;
        }

        /* ── DUES HEADER CARD ── */
        .dues-card {
            background: rgba(230, 126, 34, 0.06);
            border: 1px solid rgba(230, 126, 34, 0.2);
            border-radius: 12px;
            padding: 16px;
            margin-bottom: 24px;
            display: flex;
            flex-direction: column;
            gap: 8px;
            transition: all 0.3s ease;
        }

        .dues-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .dues-title {
            font-size: 12px;
            font-weight: 600;
            color: var(--text-muted);
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .status-badge {
            font-size: 9px;
            font-weight: 800;
            padding: 2px 8px;
            border-radius: 4px;
            letter-spacing: 0.5px;
        }

        .status-pending {
            background: rgba(230, 126, 34, 0.2);
            color: var(--secondary);
            border: 1px solid rgba(230, 126, 34, 0.3);
        }

        .status-paid {
            background: rgba(46, 204, 113, 0.2);
            color: var(--success);
            border: 1px solid rgba(46, 204, 113, 0.3);
        }

        .dues-amount {
            font-size: 26px;
            font-weight: 800;
            color: #fff;
        }

        /* ── PROPERTY DETAILS ── */
        .detail-card {
            background: rgba(255, 255, 255, 0.03);
            border: 1px solid rgba(255, 255, 255, 0.05);
            border-radius: 12px;
            padding: 20px 16px;
            margin-bottom: 24px;
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

        .detail-row {
            display: flex;
            flex-direction: column;
            gap: 3px;
        }

        .detail-label {
            font-size: 10px;
            color: var(--text-muted);
            text-transform: uppercase;
            letter-spacing: 0.5px;
            font-weight: 600;
        }

        .detail-value {
            font-size: 14px;
            font-weight: 500;
            color: #fff;
        }

        .highlight-value {
            color: var(--secondary);
            font-weight: 700;
        }

        /* ── FLOOR DETAILS GRID ── */
        .floor-section {
            margin-bottom: 24px;
        }

        .floor-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 12px;
            background: rgba(0, 0, 0, 0.15);
            border-radius: 8px;
            overflow: hidden;
            border: 1px solid rgba(255, 255, 255, 0.05);
        }

        .floor-table th {
            background: rgba(255, 255, 255, 0.04);
            color: var(--text-muted);
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            text-align: left;
            padding: 10px 12px;
            border-bottom: 1px solid rgba(255, 255, 255, 0.08);
        }

        .floor-table td {
            padding: 10px 12px;
            color: var(--text-light);
            border-bottom: 1px solid rgba(255, 255, 255, 0.04);
        }

        .floor-table tr:last-child td {
            border-bottom: none;
        }

        /* ── 3D FLIPPABLE PVC SMART CARD ── */
        .pvc-preview-container {
            perspective: 1000px;
            width: 100%;
            height: 200px;
            margin-bottom: 24px;
            cursor: pointer;
        }

        .pvc-card {
            width: 100%;
            height: 100%;
            position: relative;
            transform-style: preserve-3d;
            transition: transform 0.6s cubic-bezier(0.4, 0, 0.2, 1);
            border-radius: 16px;
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
        }

        .pvc-front, .pvc-back {
            position: absolute;
            width: 100%;
            height: 100%;
            backface-visibility: hidden;
            -webkit-backface-visibility: hidden;
            border-radius: 16px;
            padding: 18px 20px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            border: 1px solid rgba(255, 255, 255, 0.15);
        }

        .pvc-front {
            background: linear-gradient(135deg, #0f2b48 0%, #061220 100%);
        }

        .pvc-back {
            background: linear-gradient(135deg, #091a2b 0%, #030a12 100%);
            transform: rotateY(180deg);
        }

        .pvc-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
            padding-bottom: 8px;
        }

        .pvc-logo-text {
            font-size: 11px;
            font-weight: 800;
            color: #fff;
            letter-spacing: 0.5px;
        }

        .pvc-badge {
            font-size: 8px;
            font-weight: 800;
            color: var(--secondary);
            border: 1.5px solid var(--secondary);
            padding: 2px 6px;
            border-radius: 4px;
            text-transform: uppercase;
        }

        .pvc-body {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin: 12px 0;
        }

        .pvc-details {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .pvc-label {
            font-size: 8px;
            color: rgba(255, 255, 255, 0.5);
            text-transform: uppercase;
        }

        .pvc-owner {
            font-size: 14px;
            font-weight: 700;
            color: #fff;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            max-width: 240px;
        }

        .pvc-pid {
            font-size: 11px;
            font-weight: 700;
            color: var(--secondary);
        }

        .pvc-qrid {
            font-size: 8.5px;
            color: rgba(255, 255, 255, 0.4);
            font-family: monospace;
        }

        .pvc-qr-box {
            width: 60px;
            height: 60px;
            background: white;
            padding: 4px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .pvc-qr-box svg {
            width: 100%;
            height: 100%;
        }

        .pvc-footer {
            font-size: 9px;
            color: rgba(255, 255, 255, 0.4);
            text-align: center;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            border-top: 1px solid rgba(255, 255, 255, 0.08);
            padding-top: 6px;
        }

        /* ── SPINNER ── */
        .spinner {
            width: 20px;
            height: 20px;
            border: 2px solid rgba(255, 255, 255, 0.3);
            border-radius: 50%;
            border-top-color: white;
            animation: spin 0.8s linear infinite;
        }

        @keyframes spin {
            to { transform: rotate(360deg); }
        }

        /* ── RESPONSIVE DESIGN ── */
        @media (max-width: 480px) {
            .glass-card {
                padding: 24px 16px;
            }
            .container {
                margin: 20px auto;
            }
            .pvc-owner {
                max-width: 180px;
            }
        }
    </style>
</head>
<body>
    <!-- Confetti canvas -->
    <canvas id="confetti-canvas"></canvas>

    <div class="container">
        <!-- Portal Header -->
        <div class="portal-header">
            <img class="pmc-logo" src="../pmc logo.jpg" alt="Patna Nagar Nigam Logo" onerror="this.src='https://pmc.bihar.gov.in/assets/images/logo.png';" />
            <h1 class="portal-title">Patna Nagar Nigam</h1>
            <div id="portal-header-desc" class="portal-desc">Smart PVC QR Card Citizen Tax Portal</div>
        </div>

        <!-- Main Glass Card -->
        <div class="glass-card">
            
            <!-- Alert message block -->
            <div id="alert-box" class="alert alert-error hidden">
                <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="12" cy="12" r="10"></circle>
                    <line x1="12" y1="8" x2="12" y2="12"></line>
                    <line x1="12" y1="16" x2="12.01" y2="16"></line>
                </svg>
                <div id="alert-message">Error message details here</div>
            </div>

            <!-- INITIAL LOADING STATE -->
            <div id="step-loading">
                <div style="display: flex; flex-direction: column; align-items: center; justify-content: center; padding: 40px 0; gap: 15px;">
                    <div class="spinner" style="width: 32px; height: 32px; border-width: 3px; border-top-color: var(--secondary);"></div>
                    <span style="font-size: 14px; color: var(--text-muted); font-weight: 500;">Retrieving digital card data...</span>
                </div>
            </div>

            <!-- STEP 1: SCAN OR INPUT TOKEN -->
            <div id="step-scan" class="hidden">
                <div class="step-title">
                    <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
                    </svg>
                    <span>Scan Verification</span>
                </div>
                
                <div class="form-group">
                    <label class="form-label" for="manual-token">Enter QR Token or ID</label>
                    <div class="input-wrapper">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                            <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                        </svg>
                        <input type="text" id="manual-token" class="form-control" placeholder="e.g. PMC00004567 or Token ID" />
                    </div>
                </div>

                <button id="btn-check-token" class="btn">
                    <span>Lookup QR Plate</span>
                    <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="2">
                        <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
                        <circle cx="11" cy="11" r="8"></circle>
                    </svg>
                </button>
                <div class="sub-desc">Please scan the QR code fixed on your building, or type your card code to check dues.</div>
            </div>

            <!-- STEP 2: MOBILE & OTP VERIFICATION -->
            <div id="step-login" class="hidden">
                <div class="step-title">
                    <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                    </svg>
                    <span>Citizen Verification</span>
                </div>

                <div id="masked-mobile-alert" class="alert alert-warning">
                    <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path>
                    </svg>
                    <div>
                        <strong>Verification Required:</strong>
                        <div id="masked-mobile-text">This property QR is linked to mobile ending in: ****</div>
                    </div>
                </div>

                <!-- Bypass testing option -->
                <div style="text-align: right; margin-bottom: 12px;">
                    <a href="javascript:void(0)" onclick="enableTestBypass()" style="color: var(--secondary); font-size: 11px; text-decoration: none; font-weight: 500;">[Dev Bypass] Quick Test Bypass</a>
                </div>

                <div id="div-mobile-input">
                    <div class="form-group">
                        <label class="form-label" for="citizen-mobile">Registered Mobile Number</label>
                        <div class="input-wrapper">
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                <rect x="5" y="2" width="14" height="20" rx="2" ry="2"></rect>
                                <line x1="12" y1="18" x2="12.01" y2="18"></line>
                            </svg>
                            <input type="tel" id="citizen-mobile" class="form-control" placeholder="Enter 10-digit mobile number" maxlength="10" />
                        </div>
                    </div>
                    <button id="btn-send-otp" class="btn">
                        <span>Get Verification Code</span>
                        <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
                            <polyline points="22,6 12,13 2,6"></polyline>
                        </svg>
                    </button>
                </div>

                <div id="div-otp-input" class="hidden">
                    <div class="form-group">
                        <label class="form-label">Enter 6-Digit OTP</label>
                        <div class="otp-inputs">
                            <input type="text" class="otp-box" maxlength="1" oninput="moveNext(this, 1)" onkeydown="moveBack(this, 1)" id="otp-1" />
                            <input type="text" class="otp-box" maxlength="1" oninput="moveNext(this, 2)" onkeydown="moveBack(this, 2)" id="otp-2" />
                            <input type="text" class="otp-box" maxlength="1" oninput="moveNext(this, 3)" onkeydown="moveBack(this, 3)" id="otp-3" />
                            <input type="text" class="otp-box" maxlength="1" oninput="moveNext(this, 4)" onkeydown="moveBack(this, 4)" id="otp-4" />
                            <input type="text" class="otp-box" maxlength="1" oninput="moveNext(this, 5)" onkeydown="moveBack(this, 5)" id="otp-5" />
                            <input type="text" class="otp-box" maxlength="1" oninput="verifyOtpAuto(this)" onkeydown="moveBack(this, 6)" id="otp-6" />
                        </div>
                        <div class="sub-desc" style="margin-bottom: 15px;">We sent a code to your mobile. Enter it to load details.</div>
                    </div>
                    
                    <button id="btn-verify-otp" class="btn">
                        <span>Verify & Access Records</span>
                        <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="2">
                            <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                            <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                        </svg>
                    </button>
                    <button id="btn-resend-otp" class="btn btn-outline" style="margin-top: 12px;">
                        Resend OTP
                    </button>
                </div>
            </div>

            <!-- STEP 3: CITIZEN DASHBOARD -->
            <div id="step-dashboard" class="hidden">
                <div class="step-title" style="margin-bottom: 20px;">
                    <svg viewBox="0 0 24 24" width="22" height="22" fill="none" stroke="var(--success)" stroke-width="2.5">
                        <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
                        <polyline points="22 4 12 14.01 9 11.01"></polyline>
                    </svg>
                    <span style="color: var(--success);">Property Record Verified</span>
                </div>

                <!-- Dues Card Box -->
                <div id="dashboard-dues-box" class="dues-card">
                    <div class="dues-row">
                        <span class="dues-title">Current Outstanding Tax</span>
                        <span id="property-payment-status" class="status-badge">PENDING</span>
                    </div>
                    <div class="dues-row" style="margin-top: 5px;">
                        <span id="property-dues-amount" class="dues-amount">&#8377;0.00</span>
                        <button id="btn-pay-dues" class="btn" style="width: auto; padding: 10px 20px; font-size: 13px; border-radius: 8px; box-shadow: 0 4px 10px rgba(46,204,113,0.3); background: linear-gradient(135deg, var(--success) 0%, #27AE60 100%);">
                            Pay Tax Now
                        </button>
                    </div>
                </div>

                <!-- Digital PVC Card Widget (Flippable) -->
                <div class="pvc-preview-container" onclick="flipPvcCard()">
                    <div id="digital-pvc-card" class="pvc-card">
                        <!-- Front of Card -->
                        <div class="pvc-front">
                            <div class="pvc-header">
                                <span class="pvc-logo-text">PATNA NAGAR NIGAM</span>
                                <span class="pvc-badge">SMART QR</span>
                            </div>
                            <div class="pvc-body">
                                <div class="pvc-details">
                                    <span class="pvc-label">Digital Address Identity</span>
                                    <span id="pvc-card-owner" class="pvc-owner">Loading...</span>
                                    <span id="pvc-card-pid" class="pvc-pid">PID: </span>
                                    <span id="pvc-card-qrid" class="pvc-qrid">QR ID: </span>
                                </div>
                                <div class="pvc-qr-box">
                                    <svg viewBox="0 0 100 100">
                                        <!-- Mock QR layout -->
                                        <path d="M0 0 H25 V10 H10 V25 H0 Z" fill="#0D3E73"/>
                                        <path d="M75 0 H100 V25 H90 V10 H75 Z" fill="#0D3E73"/>
                                        <path d="M0 75 H10 V90 H25 V100 H0 Z" fill="#0D3E73"/>
                                        <path d="M90 75 H100 V100 H75 V90 H90 Z" fill="#0D3E73"/>
                                        <rect x="15" y="15" width="25" height="25" fill="#0D3E73"/>
                                        <rect x="15" y="60" width="25" height="25" fill="#0D3E73"/>
                                        <rect x="60" y="15" width="25" height="25" fill="#0D3E73"/>
                                        <rect x="48" y="20" width="6" height="10" fill="#0D3E73"/>
                                        <rect x="52" y="36" width="12" height="6" fill="#0D3E73"/>
                                        <rect x="20" y="46" width="8" height="8" fill="#0D3E73"/>
                                        <rect x="48" y="48" width="14" height="14" fill="#0D3E73"/>
                                        <rect x="68" y="48" width="8" height="16" fill="#0D3E73"/>
                                        <rect x="72" y="68" width="14" height="8" fill="#0D3E73"/>
                                        <rect x="52" y="76" width="12" height="12" fill="#0D3E73"/>
                                    </svg>
                                </div>
                            </div>
                            <div class="pvc-footer">Tap to view Address & Ward Details &#8635;</div>
                        </div>
                        <!-- Back of Card -->
                        <div class="pvc-back">
                            <div class="pvc-header">
                                <span class="pvc-logo-text" style="color: var(--secondary);">PROPERTY LOCATION</span>
                                <span style="font-size: 8px; color: #fff;">PATNA, BIHAR</span>
                            </div>
                            <div style="margin: 10px 0; display: flex; flex-direction: column; gap: 6px; font-size: 11px;">
                                <div style="display: flex; justify-content: space-between;"><span style="color: var(--text-muted);">Circle:</span><span id="pvc-card-circle" style="font-weight: 600;"></span></div>
                                <div style="display: flex; justify-content: space-between;"><span style="color: var(--text-muted);">Ward No:</span><span id="pvc-card-ward" style="font-weight: 600;"></span></div>
                                <div style="display: flex; justify-content: space-between;"><span style="color: var(--text-muted);">Plot Area:</span><span id="pvc-card-plot" style="font-weight: 600;"></span></div>
                                <div style="display: flex; justify-content: space-between;"><span style="color: var(--text-muted);">Built Area:</span><span id="pvc-card-built" style="font-weight: 600;"></span></div>
                            </div>
                            <div class="pvc-footer">Tap to view QR identity &#8635;</div>
                        </div>
                    </div>
                </div>

                <!-- Holding info fields -->
                <div class="detail-card">
                    <div class="detail-row">
                        <span class="detail-label">Property ID (PID)</span>
                        <span id="detail-pid" class="detail-value highlight-value"></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Owner Name</span>
                        <span id="detail-owner" class="detail-value" style="color: var(--secondary-light); font-size: 16px;"></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Father/Guardian Name</span>
                        <span id="detail-guardian" class="detail-value"></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Holding Address</span>
                        <span id="detail-address" class="detail-value" style="font-size: 13px; font-weight: 400; line-height: 1.4;"></span>
                    </div>
                </div>

                <!-- Occupancy structure section -->
                <div class="floor-section">
                    <div class="form-label" style="margin-bottom: 10px;">Building Structure Details</div>
                    <table class="floor-table">
                        <thead>
                            <tr>
                                <th>Floor</th>
                                <th>Usage</th>
                                <th>Built Area (Sqft)</th>
                                <th>Type</th>
                            </tr>
                        </thead>
                        <tbody id="floor-table-body">
                            <!-- Populated dynamically -->
                        </tbody>
                    </table>
                </div>

                <button id="btn-logout" class="btn btn-outline">
                    <span>Exit Portal</span>
                    <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path>
                        <polyline points="16 17 21 12 16 7"></polyline>
                        <line x1="21" y1="12" x2="9" y2="12"></line>
                    </svg>
                </button>
            </div>

        </div>
    </div>

    <!-- Client-side execution script -->
    <script>
        // Set Razorpay Key ID dynamically from Code-Behind
        const RAZORPAY_KEY_ID = "<%= GetRazorpayKey() %>";
        const API_BASE = './'; // Local endpoints

        let scanToken = '';
        let currentDuesAmount = 0;
        let propertyDetails = null;
        let bypassOtp = false;

        window.onload = function () {
            // Read scan token from query strings ?token= or ?value=
            const params = new URLSearchParams(window.location.search);
            const token = params.get('token') || params.get('value');
            if (token) {
                scanToken = token;
                document.getElementById('manual-token').value = token;
            }
            initFlow();
        };

        function initFlow() {
            hideAlert();
            if (!scanToken) {
                showStep('scan');
            } else {
                showStep('loading');
                validateCardToken();
            }
        }

        function showStep(step) {
            ['loading', 'scan', 'login', 'dashboard'].forEach(s => {
                const el = document.getElementById('step-' + s);
                if (el) el.classList.add('hidden');
            });
            const el = document.getElementById('step-' + step);
            if (el) el.classList.remove('hidden');
            
            const desc = document.getElementById('portal-header-desc');
            if (step === 'loading') {
                desc.innerText = 'Connecting to Patna Nagar Nigam database server...';
            } else if (step === 'scan') {
                desc.innerText = 'Scan your physical PVC QR card or enter the card number below.';
            } else if (step === 'login') {
                desc.innerText = 'Authenticate holding records to access secure tax information.';
            } else if (step === 'dashboard') {
                desc.innerText = 'Official Patna Municipal Corporation Tax Summary & Digital ID Card.';
            }
        }

        function showAlert(msg, isWarning = false) {
            const box = document.getElementById('alert-box');
            box.className = 'alert ' + (isWarning ? 'alert-warning' : 'alert-error');
            document.getElementById('alert-message').innerText = msg;
            box.classList.remove('hidden');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        function hideAlert() {
            document.getElementById('alert-box').classList.add('hidden');
        }

        // ── STEP 1: LOOKUP QR CARD ──
        document.getElementById('btn-check-token').onclick = function () {
            const token = document.getElementById('manual-token').value.trim();
            if (!token) {
                showAlert('Please enter a valid QR Token or ID.');
                return;
            }
            scanToken = token;
            validateCardToken();
        };

        async function validateCardToken() {
            hideAlert();
            const btn = document.getElementById('btn-check-token');
            const originalText = btn ? btn.innerHTML : '';
            if (btn) {
                btn.disabled = true;
                btn.innerHTML = '<div class="spinner"></div><span>Checking...</span>';
            }

            try {
                // Call server to lookup card status and get linked properties
                const res = await fetch(API_BASE + 'CardLookupHandler.ashx?value=' + encodeURIComponent(scanToken));
                const data = await res.json();

                if (data.success) {
                    // Card is valid, check status
                    const cardStatus = data.data.status.toUpperCase();
                    if (cardStatus !== 'ACTIVATED') {
                        showAlert('This QR plate is not yet activated for any property. Status: ' + cardStatus);
                        showStep('scan');
                        return;
                    }
                    
                    // Show OTP Verification stage
                    document.getElementById('masked-mobile-text').innerText = 
                        'This property QR is linked to mobile ending in: ' + (data.data.maskedMobile || '****');
                    
                    showStep('login');
                } else {
                    showAlert(data.message || 'QR Card lookup failed. Make sure the code is correct.');
                    showStep('scan');
                }
            } catch (err) {
                showAlert('Network error: ' + err.message);
                showStep('scan');
            } finally {
                if (btn) {
                    btn.disabled = false;
                    btn.innerHTML = originalText;
                }
            }
        }

        // ── STEP 2: VERIFY OTP ──
        document.getElementById('btn-send-otp').onclick = async function () {
            hideAlert();
            const mobile = document.getElementById('citizen-mobile').value.trim();
            if (mobile.length !== 10 || isNaN(mobile)) {
                showAlert('Please enter a valid 10-digit mobile number.');
                return;
            }

            const btn = document.getElementById('btn-send-otp');
            btn.disabled = true;
            btn.innerHTML = '<div class="spinner"></div><span>Sending OTP...</span>';

            try {
                const res = await fetch(API_BASE + 'CitizenPortal.ashx?action=send_otp', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ token: scanToken, mobile: mobile })
                });
                const data = await res.json();

                if (data.success) {
                    document.getElementById('div-mobile-input').classList.add('hidden');
                    document.getElementById('div-otp-input').classList.remove('hidden');
                    document.getElementById('otp-1').focus();
                } else {
                    showAlert(data.message || 'OTP request failed. Mobile number may not match records.');
                }
            } catch (err) {
                showAlert('Error sending verification code: ' + err.message);
            } finally {
                btn.disabled = false;
                btn.innerHTML = 'Get Verification Code';
            }
        };

        document.getElementById('btn-verify-otp').onclick = async function () {
            hideAlert();
            let otp = '';
            for (let i = 1; i <= 6; i++) {
                otp += document.getElementById('otp-' + i).value.trim();
            }

            if (otp.length !== 6 || isNaN(otp)) {
                showAlert('Please enter the 6-digit OTP code.');
                return;
            }

            const btn = document.getElementById('btn-verify-otp');
            btn.disabled = true;
            btn.innerHTML = '<div class="spinner"></div><span>Verifying...</span>';

            try {
                const res = await fetch(API_BASE + 'CitizenPortal.ashx?action=verify_otp', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ token: scanToken, otp: otp })
                });
                const data = await res.json();

                if (data.success) {
                    loadDashboard(data.data);
                } else {
                    showAlert(data.message || 'Verification failed. Incorrect OTP.');
                }
            } catch (err) {
                showAlert('Verification request failed: ' + err.message);
            } finally {
                btn.disabled = false;
                btn.innerHTML = 'Verify & Access Records';
            }
        };

        function enableTestBypass() {
            bypassOtp = true;
            document.getElementById('citizen-mobile').value = '9999999999';
            fetchCitizenDetailsDirectly();
        }

        async function fetchCitizenDetailsDirectly() {
            hideAlert();
            try {
                const res = await fetch(API_BASE + 'PropertyOutstandingHandler.ashx?token=' + encodeURIComponent(scanToken));
                const data = await res.json();
                if (data.success) {
                    loadDashboard(data.data);
                } else {
                    showAlert(data.message || 'Failed to load details via bypass.');
                }
            } catch (err) {
                showAlert('Bypass error: ' + err.message);
            }
        }

        // OTP inputs navigation
        window.moveNext = function (el, index) {
            if (el.value.length === 1 && index < 6) {
                document.getElementById('otp-' + (index + 1)).focus();
            }
        };

        window.moveBack = function (el, index) {
            if (event.key === 'Backspace' && el.value.length === 0 && index > 1) {
                document.getElementById('otp-' + (index - 1)).focus();
            }
        };

        window.verifyOtpAuto = function (el) {
            if (el.value.length === 1) {
                setTimeout(() => document.getElementById('btn-verify-otp').click(), 100);
            }
        };

        document.getElementById('btn-resend-otp').onclick = function () {
            for (let i = 1; i <= 6; i++) document.getElementById('otp-' + i).value = '';
            document.getElementById('div-otp-input').classList.add('hidden');
            document.getElementById('div-mobile-input').classList.remove('hidden');
        };

        // ── STEP 3: DASHBOARD & PAYMENT ──
        function loadDashboard(prop) {
            propertyDetails = prop;
            hideAlert();

            // Set text labels
            document.getElementById('detail-pid').innerText = prop.pid;
            document.getElementById('detail-owner').innerText = prop.ownerName;
            document.getElementById('detail-guardian').innerText = prop.guardianName || 'N/A';
            document.getElementById('detail-address').innerText = prop.address || 'N/A';

            // Dues Card
            const dues = parseFloat(prop.totalDues) || 0;
            currentDuesAmount = dues;
            document.getElementById('property-dues-amount').innerText = 
                '\u20B9' + dues.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            const badge = document.getElementById('property-payment-status');
            const duesCard = document.getElementById('dashboard-dues-box');
            const payBtn = document.getElementById('btn-pay-dues');

            if (dues <= 0) {
                badge.innerText = 'PAID';
                badge.className = 'status-badge status-paid';
                duesCard.style.background = 'rgba(46,204,113,0.08)';
                duesCard.style.borderColor = 'rgba(46,204,113,0.25)';
                payBtn.style.display = 'none';
            } else {
                badge.innerText = 'PENDING';
                badge.className = 'status-badge status-pending';
                duesCard.style.background = 'rgba(230,126,34,0.08)';
                duesCard.style.borderColor = 'rgba(230,126,34,0.25)';
                payBtn.style.display = 'block';
            }

            // PVC Card Details (Front)
            document.getElementById('pvc-card-owner').innerText = prop.ownerName;
            document.getElementById('pvc-card-pid').innerText = 'PID: ' + prop.pid;
            document.getElementById('pvc-card-qrid').innerText = 'QR TOKEN: ' + scanToken;

            // PVC Card Details (Back)
            document.getElementById('pvc-card-circle').innerText = prop.circle || 'Patliputra Circle';
            document.getElementById('pvc-card-ward').innerText = prop.wardNo || '15';
            document.getElementById('pvc-card-plot').innerText = (prop.plotArea || '0') + ' Sqft';
            document.getElementById('pvc-card-built').innerText = (prop.constructedArea || '0') + ' Sqft';

            // Floor structure
            const tableBody = document.getElementById('floor-table-body');
            tableBody.innerHTML = '';
            
            if (prop.floorDetails && prop.floorDetails.length > 0) {
                prop.floorDetails.forEach(f => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${f.floorNo || 'Ground Floor'}</td>
                        <td>${f.useType || 'Residential'}</td>
                        <td>${f.builtupArea || '0'}</td>
                        <td>${f.constructionType || 'Pucca'}</td>
                    `;
                    tableBody.appendChild(row);
                });
            } else {
                const emptyRow = document.createElement('tr');
                emptyRow.innerHTML = `<td colspan="4" style="text-align: center; color: var(--text-muted);">No structural details available.</td>`;
                tableBody.appendChild(emptyRow);
            }

            showStep('dashboard');
            
            if (dues <= 0) {
                triggerConfetti();
            }
        }

        function flipPvcCard() {
            const card = document.getElementById('digital-pvc-card');
            card.style.transform = card.style.transform === 'rotateY(180deg)' ? 'rotateY(0deg)' : 'rotateY(180deg)';
        }

        // ── RAZORPAY CHECKOUT INTEGRATION ──
        document.getElementById('btn-pay-dues').onclick = function () {
            if (currentDuesAmount <= 0) {
                showAlert('No outstanding dues to pay.', true);
                return;
            }

            if (typeof Razorpay === 'undefined') {
                showAlert('Razorpay SDK could not be loaded. Please check your internet connection.');
                return;
            }

            const mobile = document.getElementById('citizen-mobile').value || '9999999999';
            const owner = propertyDetails ? propertyDetails.ownerName : 'Property Owner';
            const pid = propertyDetails ? propertyDetails.pid : '';

            var options = {
                "key": RAZORPAY_KEY_ID, 
                "amount": Math.round(currentDuesAmount * 100), 
                "currency": "INR",
                "name": "Patna Nagar Nigam",
                "description": "Property Tax Payment - PID " + pid,
                "image": "https://pmc.bihar.gov.in/assets/images/logo.png",
                "prefill": {
                    "name": owner,
                    "email": "taxpayer@patna.bihar.gov.in",
                    "contact": mobile
                },
                "theme": {
                    "color": "#0D3E73"
                },
                "handler": async function (response) {
                    const paymentId = response.razorpay_payment_id;
                    await recordPaymentSuccess(paymentId);
                },
                "modal": {
                    "ondismiss": function () {
                        console.log('Payment window closed by user.');
                    }
                }
            };

            var rzp = new Razorpay(options);
            rzp.open();
        };

        async function recordPaymentSuccess(paymentId) {
            hideAlert();
            const payBtn = document.getElementById('btn-pay-dues');
            payBtn.disabled = true;
            payBtn.innerHTML = '<div class="spinner"></div><span>Recording Payment...</span>';

            try {
                const res = await fetch(API_BASE + 'ProcessPaymentHandler.ashx', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        pid: propertyDetails.pid,
                        paymentId: paymentId,
                        amount: currentDuesAmount,
                        ownerName: propertyDetails.ownerName,
                        mobileNo: document.getElementById('citizen-mobile').value || '9999999999',
                        paymentMode: 'RAZORPAY_ONLINE'
                    })
                });
                
                const data = await res.json();
                
                if (data.success) {
                    currentDuesAmount = 0;
                    if (propertyDetails) {
                        propertyDetails.totalDues = 0;
                        propertyDetails.paymentStatus = 'Paid';
                    }
                    
                    const badge = document.getElementById('property-payment-status');
                    badge.innerText = 'PAID';
                    badge.className = 'status-badge status-paid';
                    
                    document.getElementById('property-dues-amount').innerText = '₹0.00';
                    
                    const duesCard = document.getElementById('dashboard-dues-box');
                    duesCard.style.background = 'rgba(46,204,113,0.08)';
                    duesCard.style.borderColor = 'rgba(46,204,113,0.25)';
                    
                    payBtn.style.display = 'none';
                    
                    triggerConfetti();
                    showAlert('Payment completed successfully! Transaction ID: ' + paymentId, true);
                } else {
                    showAlert('Payment successful, but database update failed: ' + data.message);
                }
            } catch (err) {
                showAlert('Error updating transaction in database: ' + err.message);
            } finally {
                payBtn.disabled = false;
                payBtn.innerHTML = 'Pay Tax Now';
            }
        }

        document.getElementById('btn-logout').onclick = function () {
            document.getElementById('citizen-mobile').value = '';
            for (let i = 1; i <= 6; i++) document.getElementById('otp-' + i).value = '';
            scanToken = '';
            propertyDetails = null;
            document.getElementById('manual-token').value = '';
            showStep('scan');
        };

        function triggerConfetti() {
            const canvas = document.getElementById('confetti-canvas');
            const ctx = canvas.getContext('2d');
            canvas.width = window.innerWidth;
            canvas.height = window.innerHeight;

            const colors = ['#E67E22', '#0D3E73', '#2ECC71', '#3498DB', '#9B59B6', '#E74C3C'];
            const particles = Array.from({ length: 120 }, () => ({
                x: Math.random() * canvas.width,
                y: Math.random() * canvas.height - canvas.height,
                r: Math.random() * 6 + 4,
                d: Math.random() * canvas.height,
                color: colors[Math.floor(Math.random() * colors.length)],
                tilt: Math.random() * 10 - 5,
                tiltAngleIncremental: Math.random() * 0.07 + 0.02,
                tiltAngle: 0
            }));

            let counter = 0, rafId;
            function draw() {
                ctx.clearRect(0, 0, canvas.width, canvas.height);
                particles.forEach((p, i) => {
                    p.tiltAngle += p.tiltAngleIncremental;
                    p.y += (Math.cos(p.d) + 3 + p.r / 2) / 2;
                    p.x += Math.sin(p.tiltAngle);
                    p.tilt = Math.sin(p.tiltAngle - i / 3) * 15;
                    ctx.beginPath();
                    ctx.lineWidth = p.r;
                    ctx.strokeStyle = p.color;
                    ctx.moveTo(p.x + p.tilt + p.r / 2, p.y);
                    ctx.lineTo(p.x + p.tilt, p.y + p.tilt + p.r / 2);
                    ctx.stroke();
                });
                if (counter++ < 180) rafId = requestAnimationFrame(draw);
                else {
                    ctx.clearRect(0, 0, canvas.width, canvas.height);
                    cancelAnimationFrame(rafId);
                }
            }
            draw();
        }

        window.addEventListener('resize', () => {
            const c = document.getElementById('confetti-canvas');
            if (c) { c.width = window.innerWidth; c.height = window.innerHeight; }
        });
    </script>
</body>
</html>
