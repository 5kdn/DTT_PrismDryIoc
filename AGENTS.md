# AGENTS.md

## �T�v

�{�v���W�F�N�g�́A�uDCS�iDigital Combat Simulator�j�v�̓��{�ꉻ��Ƃ��x������Windows�A�v���P�[�V�����ł��B  
�A�v����Prism�t���[�����[�N��DryIoc�ɂ��DI�R���e�i��p����WPF�A�v���Ƃ��Đ݌v����Ă��܂��B  
�܂��AGitHub�ƘA�g���ē��{��|��t�@�C���̃_�E�����[�h�E�A�b�v���[�h�A���[�J���p�X�Ǘ������s���܂��B

�{�h�L�������g�ł́A�v���W�F�N�g���̊e�G�[�W�F���g�i�T�[�r�X�A���W���[���A�Ӗ��P�ʁj�Ƃ��̖����A�����ꏊ�ɂ��ċL�q���܂��B

---

## 1. �A�v���P�[�V�����E�G�[�W�F���g�ꗗ

### 1.1. �t�@�C������G�[�W�F���g�iFileService�j

- **�Ӗ�**  
  �t�@�C���E�f�B���N�g������̒��ۉ�����ђ񋟁B
  - ���[�J���t�@�C���̓ǂݏ���
  - �f�B���N�g���X�L����
  - DCS�|��t�@�C���̊Ǘ�

- **��Ȏ���**  
  - `MyApp.Core.Services.FileService`
  - �C���^�t�F�[�X: `MyApp.Core.Contracts.Services.IFileService`

---

### 1.2. �ݒ�Ǘ��G�[�W�F���g�iSettingsService�j

- **�Ӗ�**  
  �A�v���P�[�V�����̊e��ݒ�i��FDCS�C���X�g�[���p�X�AGitHub�A�J�E���g���Ȃǁj���i�����E�Ǘ��B

- **��Ȏ���**  
  - `MyApp.Services.SettingsService`
  - �C���^�t�F�[�X: `MyApp.Contracts.Services.ISettingsService`

---

### 1.3. GitHub�A�g�G�[�W�F���g�iGitHubService�j

- **�Ӗ�**  
  GitHub API�ioctokit���j�𗘗p�����ȉ��̋@�\��񋟁B
  - ���|�W�g���ꗗ�擾
  - �|��t�@�C���̃A�b�v���[�h�E�_�E�����[�h

- **��Ȏ���**  
  - `MyApp.Services.GitHubService`
  - �C���^�t�F�[�X: `MyApp.Contracts.Services.IGitHubService`

---

### 1.4. �e�[�}�؂�ւ��G�[�W�F���g�iThemeSelectorService�j

- **�Ӗ�**  
  �A�v���P�[�V������UI�e�[�}�i���邢/�Â��Ȃǁj�̐؂�ւ��E�Ǘ�

- **��Ȏ���**  
  - `MyApp.Services.ThemeSelectorService`
  - �C���^�t�F�[�X: `MyApp.Contracts.Services.IThemeSelectorService`

---

### 1.5. �V�X�e�����G�[�W�F���g�iSystemService�j

- **�Ӗ�**  
  OS�o�[�W������n�[�h�E�F�A���̃V�X�e�����̎擾

- **��Ȏ���**  
  - `MyApp.Services.SystemService`
  - �C���^�t�F�[�X: `MyApp.Contracts.Services.ISystemService`

---

### 1.6. ��ԉi�����G�[�W�F���g�iPersistAndRestoreService�j

- **�Ӗ�**  
  �A�v���P�[�V�����̏�ԕۑ��ƃ��X�g�A�@�\�i�E�B���h�E�ʒu�A�T�C�Y���j

- **��Ȏ���**  
  - `MyApp.Services.PersistAndRestoreService`
  - �C���^�t�F�[�X: `MyApp.Contracts.Services.IPersistAndRestoreService`

---

### 1.7. �A�v���P�[�V�������G�[�W�F���g�iApplicationInfoService�j

- **�Ӗ�**  
  �A�v���̃o�[�W�����␻�i�����A�A�v�����g�̏���

- **��Ȏ���**  
  - `MyApp.Services.ApplicationInfoService`
  - �C���^�t�F�[�X: `MyApp.Contracts.Services.IApplicationInfoService`

---

## 2. ViewModel�EUI�G�[�W�F���g

Prism��MVVM�\���ɂ��A�e��ʁi�y�[�W�j���Ƃ�ViewModel�����݂��܂��B  
�eViewModel�͏�L�T�[�r�X�Q��DI�o�R�ŗ��p���AUI���W�b�N��S�����܂��B

- ��:  
  - `MainViewModel`
  - `SettingsViewModel`
  - `TranslationUploadViewModel`
  - �Ȃ�

---

## 3. �e�X�g�G�[�W�F���g

- **�ړI**  
  �e�T�[�r�X�EViewModel�̒P�̃e�X�g�A�����e�X�g��S��

- **��Ȏ���**  
  - `MyApp.Tests`�v���W�F�N�g�z��  
  - xUnit�{Moq�Ŏ���

---

## 4. ���̑�

- **DI�Ǘ��G�[�W�F���g**  
  DryIoc��p���đS�T�[�r�X�EViewModel�̈ˑ������ꌳ�Ǘ�

---

## ���l

- �V�K�G�[�W�F���g��ǉ�����ꍇ�́A�e�Ӗ��𖾊m�����A�C���^�t�F�[�X�𕪗����Ď������邱�ƁB
- �T�[�r�X�̎����́uContracts/Interfaces�v�ƁuServices�v�t�H���_�ŕ������邱�Ƃ���������܂��B
- ViewModel��T�[�r�X�Ԃ̐Ӗ��̕��S�𖾊m�ɂ��邱�ƂŁA�ێ琫�E�e�X�g�e�Ր������コ���܂��B

---

**�ŏI�X�V**: 2025-07-02
