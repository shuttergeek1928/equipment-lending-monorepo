package com.example.demo.controller;

import com.example.demo.model.Equipment;
import com.example.demo.repository.EquipmentRepository;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@CrossOrigin(origins = "*", maxAge = 3600)
@RestController
@RequestMapping("/api/equipment")
public class EquipmentController {

    @Autowired
    private EquipmentRepository equipmentRepository;

    @GetMapping
    public List<Equipment> getAllEquipment() {
        return equipmentRepository.findAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<Equipment> getEquipmentById(@PathVariable UUID id) {
        return equipmentRepository.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }


    @PostMapping
    @PreAuthorize("hasRole('ADMIN')")
    public Equipment addEquipment(@Valid @RequestBody Equipment equipment) {


        equipment.setAvailableQuantity(equipment.getTotalQuantity());
        equipment.setAvailable(equipment.getTotalQuantity() > 0);
        return equipmentRepository.save(equipment);
    }

    @PutMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Equipment> updateEquipment(@PathVariable UUID id, @Valid @RequestBody Equipment equipmentDetails) { // <-- Change from Long
        return equipmentRepository.findById(id)
                .map(equipment -> {

                    equipment.setEquipmentName(equipmentDetails.getEquipmentName());
                    equipment.setCategory(equipmentDetails.getCategory());
                    equipment.setEquipmentCondition(equipmentDetails.getEquipmentCondition());
                    equipment.setTotalQuantity(equipmentDetails.getTotalQuantity());
                    equipment.setAvailableQuantity(equipmentDetails.getAvailableQuantity());


                    equipment.setAvailable(equipmentDetails.getAvailableQuantity() > 0);


                    return ResponseEntity.ok(equipmentRepository.save(equipment));
                })
                .orElse(ResponseEntity.notFound().build());
    }

    @DeleteMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> deleteEquipment(@PathVariable UUID id) {
        return equipmentRepository.findById(id)
                .map(equipment -> {
                    equipmentRepository.delete(equipment);
                    return ResponseEntity.ok().build();
                })
                .orElse(ResponseEntity.notFound().build());
    }
}