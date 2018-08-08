//
//  AddressView.swift
//  Walkie-Talkie
//


import Foundation
import UIKit
import ReachabilitySwift

class AddressView: UIView {
 
    @IBOutlet weak var ipAddressLab: UILabel!
    @IBOutlet var addressLabs: [UILabel]!
    @IBOutlet weak var errorLab: UILabel!
    
    func update(reachability status:Reachability.NetworkStatus, address:String?)
    {
        switch status {
        case .reachableViaWiFi:
            _ = addressLabs.map{$0.isHidden = false}
            errorLab.isHidden = true
            ipAddressLab.text = address ?? "Failed to obtain..."
            backgroundColor = UIColor.clear
            
        default:
            _ = addressLabs.map{$0.isHidden = true}
            errorLab.isHidden = false
            errorLab.text = "No WiFi connection..."
            backgroundColor = UIColor.yellow
        }
    }
}
