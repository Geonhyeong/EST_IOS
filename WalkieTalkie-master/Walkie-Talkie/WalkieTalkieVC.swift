//
//  WalkieTalkieVC.swift
//  Walkie-Talkie
//
//  Created by GeonHyeong on 18.08.22.
//  Copyright © 2018 GeonHyeong. All rights reserved.
//

import UIKit
import ReachabilitySwift
import RxReachability
import RxSwift
import RxCocoa

class WalkieTalkieVC: UIViewController {
    
    fileprivate let failedToObtainIPMess = "Failed to obtain =("
    private let toSettingsSegue = "toSettings"
    
    @IBOutlet weak var connectionIndicator: UIView!
    @IBOutlet weak var talkBtn: UIButton!
    @IBOutlet weak var disconnectBtn: UIButton!
    @IBOutlet weak var connectBtn: LoadingButton!
//    @IBOutlet weak var speakerSegmContr: UISegmentedControl!
    @IBOutlet weak var addressView: AddressView!
    @IBOutlet weak var settingsBtn: UIBarButtonItem!
    
    private (set) var audioManager:AudioManager!
    private (set) var connectionManager:ConnectionManager!
    
    fileprivate let disposeBag = DisposeBag()
    
    override func viewDidLoad()
    {
        super.viewDidLoad()

        disconnectBtn.isEnabled = false
        let stopTalkingEvents = [UIControlEvents.touchUpInside].map{disconnectBtn.rx.controlEvent($0)}.map{ $0.map{AudioManager.MicState.Off}}
        let startTalkingEvent =  talkBtn.rx.controlEvent(UIControlEvents.touchUpInside).map{AudioManager.MicState.On}
        
        let talkToggle = Observable<AudioManager.MicState>.merge(stopTalkingEvents + [startTalkingEvent]).debug()

//        let speakerToggle = speakerSegmContr.rx.selectedSegmentIndex.asObservable().flatMap { selectedIdx -> Observable<AudioManager.SpeakerType> in
//            guard let speakerType = AudioManager.SpeakerType(rawValue:selectedIdx) else {
//                return Observable.never()
//            }
//            return Observable.just(speakerType)
//        }
        
        let speakerType : Observable<AudioManager.SpeakerType> = Observable.just(AudioManager.SpeakerType(rawValue: 1)!)
        
        do {
            audioManager = try AudioManager(talkToggle, speakerToggle:speakerType, TalkBtn:talkBtn, DisconnectBtn:disconnectBtn)
            connectionManager = ConnectionManager(dataObservable: audioManager.audioDataOutput)
        }catch{
            let mes = "Failed to initiate audion manager: \(error)"
            assertionFailure(mes)
            showAlert(title: "Error", mess: mes)
        }
        
        setupNotifications()
        
        if let reachabilityStatus = appDelegate.reachability?.currentReachabilityStatus {
            
            addressView.update(reachability: reachabilityStatus, address: ConnectionManager.getWiFiAddress())
        }
        
        
        appDelegate.reachability?.rx.status.subscribe(onNext: {[weak self] status in
            self?.updateReachability(status)
        }).disposed(by: disposeBag)
        
        connectBtn.rx.tap.subscribe(onNext:{ [unowned self] in
            
            self.connectBtn.showLoading()
            do {
                try self.connectionManager.connect(receiveBlock: self.audioManager.playData)
            }catch{
                let errMess = error.localizedDescription
                self.showAlert(title: "Error", mess: errMess)
            }
        }).disposed(by: disposeBag)
        
        settingsBtn.rx.tap.subscribe(onNext:{ [unowned self] in
            self.performSegue(withIdentifier: self.toSettingsSegue, sender: nil)
        }).disposed(by: disposeBag)
    }
    

    private func setupNotifications()
    {
        NotificationCenter.default.addObserver(forName: UDP.didConnect, object: nil, queue: nil, using:connectionChanged)
        NotificationCenter.default.addObserver(forName: UDP.didDisconnect, object: nil, queue: nil, using:connectionChanged)
        NotificationCenter.default.addObserver(forName: UDP.failedToConnect, object: nil, queue:nil, using:connectionChanged)
    }
    
    private func connectionChanged(not:Notification)
    {
        self.connectBtn.hideLoading()

        switch not.name {
            case UDP.didConnect:
                toggleConnectionIndicator(enable: true)
                
                connectBtn.isEnabled = false
                connectBtn.isHidden = true
                talkBtn.backgroundColor = UIColor.green
                talkBtn.isEnabled = true
            default:
                // Failed to connect or did disconnect
                toggleConnectionIndicator(enable: false)
                if let reachability = appDelegate.reachability, case .reachableViaWiFi = reachability.currentReachabilityStatus {
                    connectBtn.isHidden = false
                    connectBtn.isEnabled = true
                }
                talkBtn.backgroundColor = UIColor.green
                talkBtn.isEnabled = false
        }
    }
    
    private func toggleConnectionIndicator(enable:Bool)
    {
        
        connectionIndicator.backgroundColor = enable ? .green : .gray
    }
    
    
    fileprivate func updateReachability(_ status:Reachability.NetworkStatus)
    {
        addressView.update(reachability: status, address: ConnectionManager.getWiFiAddress())

        if case .reachableViaWiFi = status {
            connectBtn.isEnabled = true
        }else{
            connectBtn.hideLoading()
            connectBtn.isEnabled = false
        }
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == toSettingsSegue, let settingsVC = segue.destination as? SettingsVC {
            settingsVC.connectionManager = connectionManager
        }
    }
    
    deinit {
        NotificationCenter.default.removeObserver(self)
    }
    
    @IBAction func settingBtn_disable(_ sender: UIButton) {
        settingsBtn.isEnabled = false
    }
    
    @IBAction func stop_disconnect(_ sender: UIButton) {
        self.connectionManager.disconnect()
        settingsBtn.isEnabled = true
    }
    
}

