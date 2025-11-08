package com.example.demo.controller;

import com.example.demo.model.Equipment;
import com.example.demo.repository.EquipmentRepository;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@CrossOrigin(origins = "*", maxAge = 3600)
@RestController
@RequestMapping("/api/equipment")
public class EquipmentController {

    @Autowired
    private EquipmentRepository equipmentRepository;

    // Anyone can view equipment
    @GetMapping
    public List<Equipment> getAllEquipment() {
        return equipmentRepository.findAll();
    }

    // Anyone can view a single item
    @GetMapping("/{id}")
    public ResponseEntity<Equipment> getEquipmentById(@PathVariable Long id) {
        return equipmentRepository.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    // Only an ADMIN can add new equipment
    @PostMapping
    @PreAuthorize("hasRole('ADMIN')")
    public Equipment addEquipment(@Valid @RequestBody Equipment equipment) {
        // When creating, available quantity equals total quantity
        equipment.setAvailableQuantity(equipment.getTotalQuantity());
        return equipmentRepository.save(equipment);
    }

    // Only an ADMIN can update equipment
    @PutMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Equipment> updateEquipment(@PathVariable Long id, @Valid @RequestBody Equipment equipmentDetails) {
        return equipmentRepository.findById(id)
                .map(equipment -> {
                    equipment.setName(equipmentDetails.getName());
                    equipment.setCategory(equipmentDetails.getCategory());
                    equipment.setCondition(equipmentDetails.getCondition());
                    equipment.setTotalQuantity(equipmentDetails.getTotalQuantity());
                    // You might need more complex logic here for availableQuantity
                    equipment.setAvailableQuantity(equipmentDetails.getAvailableQuantity());
                    return ResponseEntity.ok(equipmentRepository.save(equipment));
                })
                .orElse(ResponseEntity.notFound().build());
    }

    // Only an ADMIN can delete equipment
    @DeleteMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> deleteEquipment(@PathVariable Long id) {
        return equipmentRepository.findById(id)
                .map(equipment -> {
                    equipmentRepository.delete(equipment);
                    return ResponseEntity.ok().build();
                })
                .orElse(ResponseEntity.notFound().build());
    }
}