// This runs safely on EVERY page
document.addEventListener('DOMContentLoaded', function () {

    //Hide spinner if it exists
    const spinner = document.getElementById('spinner');
    if (spinner) {
        spinner.style.display = 'none';
    }

    //Mobile Sidebar Menu Logic (Runs everywhere)
    const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
    const sidebar = document.querySelector('.sidebar');

    if (mobileMenuBtn && sidebar) {
        mobileMenuBtn.addEventListener('click', function () {
            sidebar.classList.toggle('active');
        });

        // Close mobile menu when clicking outside
        document.addEventListener('click', function (event) {
            if (window.innerWidth <= 768) {
                if (!sidebar.contains(event.target) && !mobileMenuBtn.contains(event.target)) {
                    sidebar.classList.remove('active');
                }
            }
        });
    }

    //Dynamic Form Logic (Only runs if the category form exists!)
    const categorySelect = document.getElementById("itemCategory");
    if (categorySelect) {
        const dynamicFieldsContainer = document.getElementById("dynamicFieldsContainer");
        const allDynamicGroups = document.querySelectorAll(".dynamic-group");

        categorySelect.addEventListener("change", function () {
            dynamicFieldsContainer.style.display = "block";

            allDynamicGroups.forEach(group => {
                group.style.display = "none";
                const inputs = group.querySelectorAll(".dynamic-input");
                inputs.forEach(input => {
                    input.required = false;
                    input.value = "";
                    input.disabled = true;
                });
            });

            const selectedCategory = categorySelect.value;
            if (selectedCategory) {
                const targetGroup = document.getElementById("fields-" + selectedCategory);
                if (targetGroup) {
                    targetGroup.style.display = "block";
                    const targetInputs = targetGroup.querySelectorAll(".dynamic-input");
                    targetInputs.forEach(input => {
                        input.required = true;
                        input.disabled = false;
                    });
                }
            }
        });
    }

    //Custom Glass Dropdown Logic (Only runs if the custom dropdown exists!)
    const trigger = document.getElementById('customDropdownTrigger');
    if (trigger) {
        const menu = document.getElementById('customDropdownMenu');
        const hiddenSelect = document.getElementById('itemCategory');
        const selectedText = trigger.querySelector('.dropdown-selected-text');
        const customOptions = document.querySelectorAll('.custom-option');

        trigger.addEventListener('click', function (e) {
            e.stopPropagation();
            menu.classList.toggle('active');
            trigger.classList.toggle('open');
        });

        customOptions.forEach(option => {
            option.addEventListener('click', function () {
                selectedText.textContent = this.textContent;
                selectedText.style.color = "var(--primary)";
                hiddenSelect.value = this.getAttribute('data-value');
                hiddenSelect.dispatchEvent(new Event('change'));
                menu.classList.remove('active');
                trigger.classList.remove('open');
            });
        });

        document.addEventListener('click', function (e) {
            if (!trigger.contains(e.target) && !menu.contains(e.target)) {
                menu.classList.remove('active');
                trigger.classList.remove('open');
            }
        });
    }
});


// Contact Modal Logic
function openContactModal(phoneNumber) {
    // 1. Put the phone number into the text box
    document.getElementById('modalPhoneNumber').innerText = phoneNumber;

    // 2. Bonus: Automatically create a WhatsApp link using their number!
    // (Removes the leading 0 and adds Egypt's country code +20)
    let waNumber = "+20" + phoneNumber.substring(1);
    document.getElementById('whatsappBtn').href = "https://wa.me/" + waNumber;

    // 3. Show the modal
    document.getElementById('contactModal').style.display = 'flex';
}

function closeContactModal() {
    // Hide the modal
    document.getElementById('contactModal').style.display = 'none';
}

// File Upload Logic
function updateFileName(input) {
    const displaySpan = document.getElementById('fileNameDisplay');
    const labelBtn = input.nextElementSibling; // Grabs the label element

    if (input.files && input.files.length > 0) {
        // Change text to the file's name
        displaySpan.innerText = input.files[0].name;
        // Add the green gradient class
        labelBtn.classList.add('file-selected');
    } else {
        // Reset if they cancel
        displaySpan.innerText = 'Upload Item Photo (Optional)';
        labelBtn.classList.remove('file-selected');
    }
}

// Image Preview Logic
function previewImage(input) {
    const imgPreview = document.getElementById('imagePreview');
    const textContainer = document.getElementById('uploadTextContainer');
    const labelBtn = document.getElementById('uploadLabel');

    if (input.files && input.files[0]) {
        // 1. Create a temporary URL for the image
        imgPreview.src = URL.createObjectURL(input.files[0]);

        // 2. Show the image, hide the text
        imgPreview.style.display = 'block';
        textContainer.style.display = 'none';

        // 3. Clean up the padding so the image fills the box perfectly
        labelBtn.style.padding = '0.5rem';
        labelBtn.style.background = '#ffffff';
        labelBtn.style.border = '2px solid var(--primary)';
    } else {
        // If the user cancels, put everything back to normal
        imgPreview.src = '';
        imgPreview.style.display = 'none';
        textContainer.style.display = 'block';

        labelBtn.style.padding = '1.2rem';
        labelBtn.style.background = '';
        labelBtn.style.border = '';
    }
}