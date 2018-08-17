//
//  SettingsVC.swift
//  Walkie-Talkie
//

import Foundation
import UIKit
import SwiftValidator

class SettingsVC: UIViewController, UITextFieldDelegate {
    
    fileprivate let remoteIP = "192.168.35.14"
    fileprivate let validator = Validator()
    
    @IBOutlet weak var idTF: TextField!
//    @IBOutlet weak var addressTF: TextField!
    @IBOutlet weak var remotePortTF: TextField!
    @IBOutlet weak var inPortTF: TextField!
    
    var connectionManager:ConnectionManager!
    
    fileprivate lazy var accessoryView:AccessoryView = {
        let aView = AccessoryView(30)
        aView.backgroundColor = UIColor.gray
        return aView
    }()
    
    override func viewDidLoad()
    {
        super.viewDidLoad()
        self.view.addGestureRecognizer(UITapGestureRecognizer(target: self.view, action: #selector(UIView.endEditing(_:))))
        validator.registerField(idTF, rules:[RequiredRule(message: "Phone Number is empty")])
        validator.registerField(remotePortTF, rules: [RequiredRule(message: "remote port is missing"), CharacterSetRule(characterSet: .decimalDigits, message: "port may contain only numbers")])
        validator.registerField(inPortTF, rules: [RequiredRule(message: "in port is missing"), CharacterSetRule(characterSet: .decimalDigits, message: "port may contain only numbers")])
        idTF.delegate = self
    }
    
    override func viewWillAppear(_ animated: Bool)
    {
        super.viewWillAppear(animated)
        if let inPort = connectionManager.incommingPort {
            inPortTF.text = "\(inPort)"
        }
        if let rmPort = connectionManager.remotePort {
            remotePortTF.text = "\(rmPort)"
        }
        if let iD = connectionManager.ID {
            idTF.text = iD
        }
//            else if let wifiAddress = ConnectionManager.getWiFiAddress() {
//            let commonAddressPart = wifiAddress.substring(to: wifiAddress.index(after: wifiAddress.indexes(of: ".").last!))
//            idTF.text = commonAddressPart
//        }
    }
    
    
    @IBAction func saveParams(_ sender: UIButton)
    {
        connectionManager.disconnect()
        validator.validate { [unowned self] val_errs in
            
            self.connectionManager.remotePort = Int(self.remotePortTF.text!)
            self.connectionManager.incommingPort = Int(self.inPortTF.text!)
            self.connectionManager.remoteAddress = remoteIP
            self.connectionManager.ID = self.idTF.text
            
            if val_errs.count > 0  {
                let err_strs = val_errs.map{ $1.errorMessage}.joined(separator: ", ")
                let finalMess = "Holly molly! Validation failed =( Probably, you won't be able to establish connection. Here are some clues for you: " + "\(err_strs)."
                self.showAlert(title: "Oppps", mess: finalMess)
                return
            }
                
            _ = self.navigationController?.popViewController(animated: true)
            
        }
    }
    
}


extension SettingsVC{
    
 
    func textFieldShouldReturn(_ textField: UITextField) -> Bool {
        if let nextTF = (textField as? TextField)?.nextResp{
            nextTF.becomeFirstResponder()
        } else{
            textField.resignFirstResponder()
        }
        return false
    }
    
    
    func textFieldShouldBeginEditing(_ textField: UITextField) -> Bool
    {
        textField.inputAccessoryView = accessoryView
        accessoryView.textField = textField as? TextField
        return true
    }
}
